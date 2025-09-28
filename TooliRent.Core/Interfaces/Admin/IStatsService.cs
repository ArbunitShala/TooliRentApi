using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;

namespace TooliRent.Core.Interfaces.Admin
{
    public interface IStatsService
    {
        Task<StatsResponse> GetDashboardAsync(CancellationToken ct);
    }
}
