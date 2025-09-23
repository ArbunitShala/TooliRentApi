using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Auth;
using TooliRent.Core.Interfaces.Auth;
using TooliRent.Core.Models.Auth;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Infrastructure.Auth
{
    public sealed class IdentityAuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IdentityAuthRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<(Guid Id, string Email, bool IsActive)?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return null;
            return (user.Id, user.Email!, user.IsActive);
        }

        public async Task<(string Email, bool IsActive)?> FindByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return null;
            return (user.Email!, user.IsActive);
        }

        public async Task<string?> GetEmailByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Email;
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

        // register member ---------
        public async Task<(bool Success, string? Error)> CreateMemberAsync(
        string email, string password, string firstName, string lastName)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null)
                return (false, "E-postadressen används redan.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true,
                IsActive = true
            };

            var res = await _userManager.CreateAsync(user, password);
            if (!res.Succeeded)
                return (false, string.Join("; ", res.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Member");
            return (true, null);
        }

        // Refresh tokens ---------
        public async Task StoreRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresUtc)
        {
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAtUtc = expiresUtc
            });
            await _db.SaveChangesAsync();
        }

        public async Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash)
            => await _db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

        public async Task RevokeAndReplaceAsync(string oldTokenHash, string newTokenHash, DateTime newExpiresUtc)
        {
            var old = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == oldTokenHash);
            if (old is null) return;

            old.RevokedAtUtc = DateTime.UtcNow;
            old.ReplacedByTokenHash = newTokenHash;

            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = old.UserId,
                TokenHash = newTokenHash,
                ExpiresAtUtc = newExpiresUtc
            });

            await _db.SaveChangesAsync();
        }

        public async Task<bool> RevokeRefreshTokenAsync(string tokenHash)
        {
            var tok = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
            if (tok is null) return false;
            if (tok.RevokedAtUtc is null) tok.RevokedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
