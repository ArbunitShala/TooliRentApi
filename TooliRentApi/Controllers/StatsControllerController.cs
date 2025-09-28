using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;

namespace TooliRentApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/stats")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _svc;
        public StatsController(IStatsService svc) => _svc = svc;

        [HttpGet("dashboard")]
        public async Task<ActionResult<StatsResponse>> GetDashboard(CancellationToken ct)
            => Ok(await _svc.GetDashboardAsync(ct));
    }
}
