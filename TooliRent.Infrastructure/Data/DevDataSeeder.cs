using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Infrastructure.Data
{
    public static class DevDataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Kategorier (läggs bara om de saknas)
            var categoriesToEnsure = new[]
            {
            new Category { Name = "Saws",    Description = "Sågar" },
            new Category { Name = "Drills",  Description = "Borrmaskiner" },
            new Category { Name = "Sanders", Description = "Slipmaskiner" }
        };

            foreach (var c in categoriesToEnsure)
            {
                if (!await db.Categories.AnyAsync(x => x.Name == c.Name))
                    db.Categories.Add(new Category { Name = c.Name, Description = c.Description });
            }
            if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync();

            // Hämta kategori-Id:n
            var sawsId = await db.Categories.Where(c => c.Name == "Saws").Select(c => c.Id).FirstAsync();
            var drillsId = await db.Categories.Where(c => c.Name == "Drills").Select(c => c.Id).FirstAsync();
            var sandersId = await db.Categories.Where(c => c.Name == "Sanders").Select(c => c.Id).FirstAsync();

            // Verktyg som ska finnas (läggs endast om de saknas namn-mässigt)
            var toolsToEnsure = new[]
            {
            // Saws
            new Tool { Name = "Makita Circular Saw", CategoryId = sawsId },
            new Tool { Name = "Bosch Jigsaw", CategoryId = sawsId },

            // Drills
            new Tool { Name = "DeWalt Impact Drill", CategoryId = drillsId },
            new Tool { Name = "Makita Drill 12V",  CategoryId = drillsId },

            // Sanders (NYTT)
            new Tool { Name = "Makita Orbital Sander", CategoryId = sandersId },
            new Tool { Name = "Bosch Belt Sander", CategoryId = sandersId }
        };

            foreach (var t in toolsToEnsure)
            {
                if (!await db.Tools.AnyAsync(x => x.Name == t.Name))
                    db.Tools.Add(new Tool { Name = t.Name, CategoryId = t.CategoryId });
            }

            if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync();
        }
    }
}
