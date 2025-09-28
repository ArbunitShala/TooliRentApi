using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/tools")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ToolsAdminController : ControllerBase
    {
        private readonly IToolAdminService _svc;
        public ToolsAdminController(IToolAdminService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ToolAdminDto>>> List(CancellationToken ct)
            => Ok(await _svc.ListAsync(ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ToolAdminDto>> Get(Guid id, CancellationToken ct)
            => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<ToolAdminDto>> Create(CreateToolAdminRequest req, CancellationToken ct)
        { var created = await _svc.CreateAsync(req, ct); return CreatedAtAction(nameof(Get), new { id = created.Id }, created); }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ToolAdminDto>> Update(Guid id, UpdateToolAdminRequest req, CancellationToken ct)
            => Ok(await _svc.UpdateAsync(id, req, ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        { await _svc.DeleteAsync(id, ct); return NoContent(); }
    }
}
