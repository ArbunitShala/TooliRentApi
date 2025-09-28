using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class UsersAdminController : ControllerBase
    {
        private readonly IUserAdminService _svc;
        public UsersAdminController(IUserAdminService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserAdminDto>>> List(CancellationToken ct)
            => Ok(await _svc.ListAsync(ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserAdminDto>> Get(Guid id, CancellationToken ct)
            => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPatch("{id:guid}/active")]
        public async Task<ActionResult<UserAdminDto>> SetActive(Guid id, SetUserActiveRequest req, CancellationToken ct)
            => Ok(await _svc.SetActiveAsync(id, req.IsActive, ct));

        [HttpPatch("{id:guid}/role")]
        public async Task<ActionResult<UserAdminDto>> SetRole(Guid id, SetUserRoleRequest req, CancellationToken ct)
            => Ok(await _svc.SetRoleAsync(id, req.Role, ct));
    }
}
