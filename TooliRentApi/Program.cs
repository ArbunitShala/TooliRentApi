using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using TooliRent.Core.DTOs.Bookings;
using TooliRent.Core.DTOs.Catalog;
using TooliRent.Core.Interfaces.Admin;
using TooliRent.Core.Interfaces.Bookings;
using TooliRent.Core.Interfaces.Catalog;
using TooliRent.Infrastructure.Auth;
using TooliRent.Infrastructure.Bookings;
using TooliRent.Infrastructure.Catalog; // ToolRepository
using TooliRent.Infrastructure.Data;
using TooliRent.Services.Services.Admin;
using TooliRent.Services.Services.Bookings;
using TooliRent.Services.Services.Bookings.Mapping;
using TooliRent.Services.Services.Bookings.Validation;
using TooliRent.Services.Services.Catalog; // ToolService
using TooliRent.Services.Services.Catalog.Mapping;
using TooliRent.Services.Services.Catalog.Validation;

namespace TooliRentApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // hämta konfiguration från appsettings.json
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            // CORS policy
            const string CorsPolicy = "DefaultCors";

            builder.Services.AddCors(o =>
            {
                o.AddPolicy(CorsPolicy, p =>
                    p.WithOrigins(
                        "https://toolirent-api.azurewebsites.net", // din Azure-app
                        "https://localhost:7044",                  // lokal https
                        "http://localhost:5019"                    // lokal http
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });

            // JWT i swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TooliRent API", Version = "v1" });

                // Lås Swagger till den bas-URL du faktiskt kör
                c.AddServer(new OpenApiServer { Url = "http://localhost:5019" });
                // När du kör https-profilen, byt till denna istället:
                // c.AddServer(new OpenApiServer { Url = "https://localhost:7044" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey, //ApiKey
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = " Bearer <Klistra in din JWT här>"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                };
            });

            // stänger av cookies redirect
            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
                o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
            });

            // Add services to the container.

            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<
                TooliRent.Core.Interfaces.Auth.IAuthRepository,
                TooliRent.Infrastructure.Auth.IdentityAuthRepository>();

            builder.Services.AddScoped<
                TooliRent.Core.Interfaces.Auth.IJwtTokenGenerator,
                TooliRent.Infrastructure.Auth.JwtTokenGenerator>();

            builder.Services.AddScoped<
                TooliRent.Core.Interfaces.Auth.IAuthService,
                TooliRent.Services.Services.Auth.AuthService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Catalog: repo + service
            builder.Services.AddScoped<IToolRepository, ToolRepository>();
            builder.Services.AddScoped<IToolService, ToolService>();

            // FluentValidation registrerar konkret validator -----
            builder.Services.AddScoped<IValidator<ToolQueryParams>, ToolQueryParamsValidator>();

            // AutoMapper 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Bookings
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IValidator<CreateBookingRequest>, CreateBookingRequestValidator>();
            builder.Services.AddScoped<IValidator<CheckoutRequest>, CheckoutRequestValidator>();
            builder.Services.AddScoped<IValidator<ReturnRequest>, ReturnRequestValidator>();

            // Admin operations
            builder.Services.AddScoped<ICategoryAdminService, CategoryAdminService>();
            builder.Services.AddScoped<IToolAdminService, ToolAdminService>();
            builder.Services.AddScoped<IUserAdminService, UserAdminService>();
            builder.Services.AddScoped<IStatsService, StatsService>();


            var app = builder.Build();

            //  minimal pipeline för Azure

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection(); 
            app.UseCors(CorsPolicy); 
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // migrering/seed precis före start
            await AppSeeder.MigrateAndSeedAsync(app.Services);

            app.Run();
        }
    }
}
