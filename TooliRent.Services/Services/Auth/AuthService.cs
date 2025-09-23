using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using TooliRent.Core.DTOs.Auth;
using TooliRent.Core.Interfaces.Auth;

namespace TooliRent.Services.Services.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IConfiguration _cfg;

        public AuthService(IAuthRepository repo, IJwtTokenGenerator jwt, IConfiguration cfg)
        {
            _repo = repo;
            _jwt = jwt;
            _cfg = cfg;
        }

        public async Task<(bool Success, LoginResponse? Data, string? Error)> LoginAsync(LoginRequest request)
        {
            var u = await _repo.FindByEmailAsync(request.Email);
            if (u is null || !u.Value.IsActive) return (false, null, "Fel e-post eller lösenord.");

            var ok = await _repo.CheckPasswordAsync(u.Value.Id, request.Password);
            if (!ok) return (false, null, "Fel e-post eller lösenord.");

            var roles = await _repo.GetRolesAsync(u.Value.Id);
            var access = _jwt.GenerateToken(u.Value.Id, u.Value.Email, roles, out var accessExpUtc);

            var refresh = GenerateRefreshToken();
            var refreshHash = Hash(refresh);
            var days = int.TryParse(_cfg["RefreshToken:Days"], out var d) ? d : 14;
            await _repo.StoreRefreshTokenAsync(u.Value.Id, refreshHash, DateTime.UtcNow.AddDays(days));

            return (true, new LoginResponse(access, accessExpUtc, refresh), null);
        }

        public async Task<(bool Success, LoginResponse? Data, string? Error)> RefreshAsync(RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return (false, null, "Saknar refresh-token.");

            var oldHash = Hash(request.RefreshToken);
            var stored = await _repo.FindRefreshTokenAsync(oldHash);
            if (stored is null) return (false, null, "Ogiltigt refresh-token.");
            if (stored.RevokedAtUtc is not null) return (false, null, "Token är spärrat.");
            if (stored.ExpiresAtUtc <= DateTime.UtcNow) return (false, null, "Token har gått ut.");

            var email = await _repo.GetEmailByIdAsync(stored.UserId);
            if (email is null) return (false, null, "Användare saknas.");

            var roles = await _repo.GetRolesAsync(stored.UserId);
            var access = _jwt.GenerateToken(stored.UserId, email, roles, out var accessExpUtc);

            var newRefresh = GenerateRefreshToken();
            var newHash = Hash(newRefresh);
            var days = int.TryParse(_cfg["RefreshToken:Days"], out var d) ? d : 14;
            await _repo.RevokeAndReplaceAsync(oldHash, newHash, DateTime.UtcNow.AddDays(days));

            return (true, new LoginResponse(access, accessExpUtc, newRefresh), null);
        }

        public async Task<(bool Success, string? Error)> LogoutAsync(RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return (false, "Saknar refresh-token.");

            var ok = await _repo.RevokeRefreshTokenAsync(Hash(request.RefreshToken));
            return ok ? (true, null) : (false, "Ogiltigt refresh-token.");
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
            => await _repo.CreateMemberAsync(request.Email, request.Password, request.FirstName, request.LastName);

        private static string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        private static string Hash(string value)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}
