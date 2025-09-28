using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;
using TooliRent.Infrastructure.Auth;

namespace TooliRent.Services.Services.Admin
{
    public class UserAdminService : IUserAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAdminService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserAdminDto?> GetAsync(Guid id, CancellationToken ct)
        {
            var u = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (u is null) return null;

            var role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? string.Empty;
            return new UserAdminDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                Role = role,
                IsActive = u.IsActive
            };
        }

        public async Task<IReadOnlyList<UserAdminDto>> ListAsync(CancellationToken ct)
        {
            var users = await _userManager.Users.AsNoTracking().OrderBy(x => x.Email!).ToListAsync(ct);
            var list = new List<UserAdminDto>(users.Count);

            foreach (var u in users)
            {
                var role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? string.Empty;
                list.Add(new UserAdminDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    Role = role,
                    IsActive = u.IsActive
                });
            }
            return list;
        }

        public async Task<UserAdminDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct)
        {
            var u = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id, ct)
                    ?? throw new KeyNotFoundException("User not found");

            u.IsActive = isActive;
            await _userManager.UpdateAsync(u);

            var role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? string.Empty;
            return new UserAdminDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                Role = role,
                IsActive = u.IsActive
            };
        }

        public async Task<UserAdminDto> SetRoleAsync(Guid id, string role, CancellationToken ct)
        {
            var u = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id, ct)
                    ?? throw new KeyNotFoundException("User not found");

            var current = await _userManager.GetRolesAsync(u);
            if (current.Any())
                await _userManager.RemoveFromRolesAsync(u, current);

            await _userManager.AddToRoleAsync(u, role);

            return new UserAdminDto
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                Role = role,
                IsActive = u.IsActive
            };
        }
    }
}
