using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Auth;

namespace TooliRent.Core.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<(bool Success, LoginResponse? Data, string? Error)> LoginAsync(LoginRequest request);
        Task<(bool Success, LoginResponse? Data, string? Error)> RefreshAsync(RefreshRequest request);
        Task<(bool Success, string? Error)> LogoutAsync(RefreshRequest request);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request);
    }
}
