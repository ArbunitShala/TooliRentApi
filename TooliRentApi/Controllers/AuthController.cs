using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooliRent.Core.DTOs.Auth;
using TooliRent.Core.Interfaces;
using TooliRent.Core.Interfaces.Auth;

namespace TooliRentApi.WebAPI.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var res = await _auth.LoginAsync(dto);
            return res.Success ? Ok(res.Data) : Unauthorized(new { error = res.Error });
        }

        // Enkel refresh – kräver att nuvarande token fortfarande är giltigt
        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var sid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(sid)) return Unauthorized();
            var userId = Guid.Parse(sid);

            var res = await _auth.RefreshAsync(userId);
            return res.Success ? Ok(res.Data) : Unauthorized(new { error = res.Error });
        }
    }
}
