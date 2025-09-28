using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;

namespace TooliRent.Core.Interfaces.Admin
{
    public interface ICategoryAdminService
    {
        Task<CategoryDto> CreateAsync(CreateCategoryRequest req, CancellationToken ct);
        Task<CategoryDto?> GetAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken ct);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest req, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
