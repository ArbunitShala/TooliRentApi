using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces.Catalog;
using TooliRent.Core.Models.Catalog;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Infrastructure.Catalog
{
    // EF Core-implementation utan paging.
    public class ToolRepository : IToolRepository
    {
        private readonly ApplicationDbContext _db;

        public ToolRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _db.Set<Tool>()
                .AsNoTracking()
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<IReadOnlyList<Tool>> SearchAsync(
            string? search,
            Guid? categoryId,
            ToolStatus? status,
            bool? onlyAvailable,
            string? sortBy,
            bool sortDesc,
            CancellationToken ct = default)
        {
            IQueryable<Tool> q = _db.Set<Tool>()
                .AsNoTracking()
                .Include(t => t.Category);

            // Fritext (namn, beskrivning, kategori)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(t =>
                    t.Name.Contains(s) ||
                    (t.Description != null && t.Description.Contains(s)) ||
                    (t.Category != null && t.Category.Name.Contains(s)));
            }

            // Filter
            if (categoryId.HasValue) q = q.Where(t => t.CategoryId == categoryId.Value);
            if (status.HasValue) q = q.Where(t => t.Status == status.Value);
            if (onlyAvailable == true) q = q.Where(t => t.Status == ToolStatus.Available);

            // Sortering
            q = (sortBy ?? "name").ToLowerInvariant() switch
            {
                "status" => sortDesc ? q.OrderByDescending(t => t.Status) : q.OrderBy(t => t.Status),
                "category" => sortDesc ? q.OrderByDescending(t => t.Category!.Name) : q.OrderBy(t => t.Category!.Name),
                _ => sortDesc ? q.OrderByDescending(t => t.Name) : q.OrderBy(t => t.Name),
            };

            return await q.ToListAsync(ct);
        }
    }
}
