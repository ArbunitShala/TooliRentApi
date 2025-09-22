using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces.Auth;

namespace TooliRent.Infrastructure.Auth
{
    public sealed class IdentityAuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityAuthRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(Guid Id, string Email, bool IsActive)?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return null;
            return (user.Id, user.Email!, user.IsActive);
        }

        public async Task<(string Email, bool IsActive)?> FindByIdAsync(Guid userId) // för refresh-token
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return null;
            return (user.Email!, user.IsActive);
        }

        public async Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Array.Empty<string>();
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
