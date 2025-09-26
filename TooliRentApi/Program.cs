using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TooliRent.Core.DTOs.Catalog;
using TooliRent.Core.Interfaces.Catalog;
using TooliRent.Infrastructure.Auth;
using TooliRent.Infrastructure.Catalog; // ToolRepository
using TooliRent.Infrastructure.Data;
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
            const string CorsPolicy = "Dev";

            builder.Services.AddCors(o =>
            {
                o.AddPolicy(CorsPolicy, p =>
                    p.WithOrigins(
                        "https://localhost:7044", // HTTPS-porten
                        "http://localhost:5019"   // HTTP-porten
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });

            // JWT i swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TooliRent API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Skriv: Bearer {ditt JWT-token}"
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
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
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

            // AutoMapper – scanna profiler (välj EN av raderna) -----
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

            await AppSeeder.MigrateAndSeedAsync(app.Services);

            app.Run();
        }
    }
}
