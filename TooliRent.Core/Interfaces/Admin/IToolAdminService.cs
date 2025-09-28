using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;

namespace TooliRent.Core.Interfaces.Admin
{
    public interface IToolAdminService
    {
        Task<ToolAdminDto> CreateAsync(CreateToolAdminRequest req, CancellationToken ct);
        Task<ToolAdminDto?> GetAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<ToolAdminDto>> ListAsync(CancellationToken ct);
        Task<ToolAdminDto> UpdateAsync(Guid id, UpdateToolAdminRequest req, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
    }
}
