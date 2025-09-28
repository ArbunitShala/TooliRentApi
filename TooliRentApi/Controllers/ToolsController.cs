using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Core.DTOs.Catalog;
using TooliRent.Core.Interfaces.Catalog;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Member,Admin")]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _service;
        private readonly IValidator<ToolQueryParams> _validator;

        public ToolsController(IToolService service, IValidator<ToolQueryParams> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTools([FromQuery] ToolQueryParams query, CancellationToken ct)
        {
            var validation = await _validator.ValidateAsync(query, ct);
            if (!validation.IsValid)
            {
                foreach (var e in validation.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);

                return ValidationProblem(ModelState);
            }

            var list = await _service.SearchAsync(query, ct);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToolById([FromRoute] Guid id, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(id, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
    }
}