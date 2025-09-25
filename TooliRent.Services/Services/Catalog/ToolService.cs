using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Catalog;
using TooliRent.Core.Interfaces.Catalog;

namespace TooliRent.Services.Services.Catalog
{
    // Service-lager utan paging
    public class ToolService : IToolService
    {
        private readonly IToolRepository _repo;
        private readonly IMapper _mapper;

        public ToolService(IToolRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ToolListItemDto>> SearchAsync(ToolQueryParams query, CancellationToken ct = default)
        {
            var items = await _repo.SearchAsync(
                query.Search,
                query.CategoryId,
                query.Status,
                query.OnlyAvailable,
                query.SortBy,
                query.SortDesc,
                ct);

            return items.Select(t => _mapper.Map<ToolListItemDto>(t)).ToList();
        }

        public async Task<ToolDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            return entity is null ? null : _mapper.Map<ToolDetailsDto>(entity);
        }
    }
}
