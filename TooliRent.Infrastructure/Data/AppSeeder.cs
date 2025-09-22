using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Infrastructure.Data
{
    public static class AppSeeder
    {
        public static async Task MigrateAndSeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Kör ev. väntande migrationer
            await db.Database.MigrateAsync();

            // Seed domändata
            await DevDataSeeder.SeedAsync(db);

            // Seed roller/användare
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        }
    }
}
