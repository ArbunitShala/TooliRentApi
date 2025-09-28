using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Member,Admin")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var res = await _auth.RegisterAsync(dto);
            return res.Success ? StatusCode(201) : BadRequest(new { error = res.Error });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var res = await _auth.LoginAsync(dto);
            return res.Success ? Ok(res.Data) : Unauthorized(new { error = res.Error });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest dto)
        {
            var res = await _auth.RefreshAsync(dto);
            return res.Success ? Ok(res.Data) : Unauthorized(new { error = res.Error });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest dto)
        {
            var res = await _auth.LogoutAsync(dto);
            return res.Success ? NoContent() : BadRequest(new { error = res.Error });
        }
    }
}
