using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;

namespace TooliRent.Core.Interfaces.Admin
{
    public interface IUserAdminService
    {
        Task<UserAdminDto?> GetAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<UserAdminDto>> ListAsync(CancellationToken ct);
        Task<UserAdminDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct);
        Task<UserAdminDto> SetRoleAsync(Guid id, string role, CancellationToken ct);
    }
}
