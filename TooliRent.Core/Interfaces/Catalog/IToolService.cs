using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Catalog;

namespace TooliRent.Core.Interfaces.Catalog
{
    public interface IToolService
    {
        Task<IReadOnlyList<ToolListItemDto>> SearchAsync(ToolQueryParams query, CancellationToken ct = default);
        Task<ToolDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
