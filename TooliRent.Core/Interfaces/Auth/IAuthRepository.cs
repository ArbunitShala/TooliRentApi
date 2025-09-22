using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<(Guid Id, string Email, bool IsActive)?> FindByEmailAsync(string email);
        Task<(string Email, bool IsActive)?> FindByIdAsync(Guid userId);   // ny
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
    }
}
