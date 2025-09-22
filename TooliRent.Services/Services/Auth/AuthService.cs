using TooliRent.Core.DTOs.Auth;
using TooliRent.Core.Interfaces.Auth;

namespace TooliRent.Services.Services.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(IAuthRepository repo, IJwtTokenGenerator jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<(bool Success, LoginResponse? Data, string? Error)> LoginAsync(LoginRequest request)
        {
            var user = await _repo.FindByEmailAsync(request.Email);
            if (user is null) return (false, null, "Fel e-post eller lösenord.");

            if (!user.Value.IsActive) return (false, null, "Kontot är inaktiverat.");

            var ok = await _repo.CheckPasswordAsync(user.Value.Id, request.Password);
            if (!ok) return (false, null, "Fel e-post eller lösenord.");

            var roles = await _repo.GetRolesAsync(user.Value.Id);
            var token = _jwt.GenerateToken(user.Value.Id, user.Value.Email, roles, out var expUtc);

            return (true, new LoginResponse(token, expUtc), null);
        }

        // enkel refresh: genererar ett nytt access-token åt den redan inloggade användaren
        public async Task<(bool Success, LoginResponse? Data, string? Error)> RefreshAsync(Guid userId)
        {
            var info = await _repo.FindByIdAsync(userId);
            if (info is null || !info.Value.IsActive)
                return (false, null, "Konto saknas eller är inaktivt.");

            var roles = await _repo.GetRolesAsync(userId);
            var token = _jwt.GenerateToken(userId, info.Value.Email, roles, out var expUtc);

            return (true, new LoginResponse(token, expUtc), null);
        }
    }
}
