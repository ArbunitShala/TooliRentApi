using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Auth;
using TooliRent.Core.Models.Auth;

namespace TooliRent.Core.Interfaces.Auth
{
    public interface IAuthRepository
    {
        // Users
        Task<(Guid Id, string Email, bool IsActive)?> FindByEmailAsync(string email);
        Task<(string Email, bool IsActive)?> FindByIdAsync(Guid userId);
        Task<string?> GetEmailByIdAsync(Guid userId);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
        Task<(bool Success, string? Error)> CreateMemberAsync(string email, string password, string firstName, string lastName);

        // Refresh tokens
        Task StoreRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresUtc);
        Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash);
        Task RevokeAndReplaceAsync(string oldTokenHash, string newTokenHash, DateTime newExpiresUtc);
        Task<bool> RevokeRefreshTokenAsync(string tokenHash);
    }
}
