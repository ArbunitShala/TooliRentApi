using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Services.Services.Admin
{
    public class ToolAdminService : IToolAdminService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public ToolAdminService(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        public async Task<ToolAdminDto> CreateAsync(CreateToolAdminRequest req, CancellationToken ct)
        {
            var catExists = await _db.Categories.AnyAsync(c => c.Id == req.CategoryId, ct);
            if (!catExists) throw new KeyNotFoundException("Category not found");

            var entity = _mapper.Map<TooliRent.Core.Models.Catalog.Tool>(req);
            _db.Tools.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await _db.Tools.Where(t => t.Id == entity.Id)
                .ProjectTo<ToolAdminDto>(_mapper.ConfigurationProvider)
                .SingleAsync(ct);
        }

        public async Task<ToolAdminDto?> GetAsync(Guid id, CancellationToken ct)
            => await _db.Tools.Where(t => t.Id == id)
                .ProjectTo<ToolAdminDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(ct);

        public async Task<IReadOnlyList<ToolAdminDto>> ListAsync(CancellationToken ct)
            => await _db.Tools.OrderBy(t => t.Name)
                .ProjectTo<ToolAdminDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

        public async Task<ToolAdminDto> UpdateAsync(Guid id, UpdateToolAdminRequest req, CancellationToken ct)
        {
            var e = await _db.Tools.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, ct)
                    ?? throw new KeyNotFoundException($"Tool {id} not found");

            var catExists = await _db.Categories.AnyAsync(c => c.Id == req.CategoryId, ct);
            if (!catExists) throw new KeyNotFoundException("Category not found");

            _mapper.Map(req, e);
            await _db.SaveChangesAsync(ct);

            return await _db.Tools.Where(t => t.Id == id)
                .ProjectTo<ToolAdminDto>(_mapper.ConfigurationProvider)
                .SingleAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var e = await _db.Tools.FindAsync(new object?[] { id }, ct);
            if (e is null) return;
            _db.Tools.Remove(e);
            await _db.SaveChangesAsync(ct);
        }
    }
}
