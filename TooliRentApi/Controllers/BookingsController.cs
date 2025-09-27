using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooliRent.Core.DTOs.Bookings;
using TooliRent.Core.Interfaces.Bookings;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Member,Admin")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingsController(IBookingService service) => _service = service;

        // Robust hämtning av användar-id (klarar olika claim-namn och null)
        private bool TryGetUserId(out Guid userId)
        {
            userId = default;
            var id =
                User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User?.FindFirstValue("sub") ??
                User?.FindFirstValue("uid") ??
                User?.FindFirstValue("id");
            return Guid.TryParse(id, out userId);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest req, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            try
            {
                var dto = await _service.CreateAsync(userId, req, ct);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (ValidationException ex)
            {
                foreach (var e in ex.Errors)
                    ModelState.AddModelError(e.PropertyName ?? "request", e.ErrorMessage);
                return ValidationProblem(ModelState);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Mina bokingar
        //[HttpGet("my")]
        [HttpGet("~/api/Bookings/my")]      // ABSOLUT path
        public async Task<IActionResult> GetMyBookings(CancellationToken ct)
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");
            if (!Guid.TryParse(id, out var userId)) return Unauthorized();

            var list = await _service.GetMyBookingsAsync(userId, ct);
            return Ok(list);
        }

        //public async Task<IActionResult> GetMyBookings(CancellationToken ct)
        //{
        //    if (!TryGetUserId(out var userId)) return Unauthorized();
        //    var list = await _service.GetMyBookingsAsync(userId, ct);
        //    return Ok(list);
        //}

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var dto = await _service.GetByIdAsync(userId, id, User.IsInRole("Admin"), ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken ct)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var ok = await _service.CancelAsync(userId, id, User.IsInRole("Admin"), ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
