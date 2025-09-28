using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;
using TooliRent.Core.Models.Catalog;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Services.Services.Admin
{
    public class CategoryAdminService : ICategoryAdminService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CategoryAdminService(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest req, CancellationToken ct)
        {
            var e = _mapper.Map<Category>(req);
            _db.Categories.Add(e);
            await _db.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(e);
        }

        public async Task<CategoryDto?> GetAsync(Guid id, CancellationToken ct)
            => (await _db.Categories.FindAsync(new object?[] { id }, ct)) is { } e ? _mapper.Map<CategoryDto>(e) : null;

        public async Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken ct)
            => _mapper.Map<IReadOnlyList<CategoryDto>>(await _db.Categories.OrderBy(x => x.Name).ToListAsync(ct));

        public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest req, CancellationToken ct)
        {
            var e = await _db.Categories.FindAsync(new object?[] { id }, ct)
                    ?? throw new KeyNotFoundException($"Category {id} not found");
            _mapper.Map(req, e);
            await _db.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(e);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var e = await _db.Categories.FindAsync(new object?[] { id }, ct);
            if (e is null) return;
            _db.Categories.Remove(e);
            await _db.SaveChangesAsync(ct);
        }
    }
}
