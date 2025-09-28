using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class CategoriesAdminController : ControllerBase
    {
        private readonly ICategoryAdminService _svc;
        public CategoriesAdminController(ICategoryAdminService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> List(CancellationToken ct)
            => Ok(await _svc.ListAsync(ct));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryDto>> Get(Guid id, CancellationToken ct)
            => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest req, CancellationToken ct)
        { var created = await _svc.CreateAsync(req, ct); return CreatedAtAction(nameof(Get), new { id = created.Id }, created); }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoryDto>> Update(Guid id, UpdateCategoryRequest req, CancellationToken ct)
            => Ok(await _svc.UpdateAsync(id, req, ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        { await _svc.DeleteAsync(id, ct); return NoContent(); }
    }
}
