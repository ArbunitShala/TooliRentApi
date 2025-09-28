using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Admin
{
    public record UsageSummaryDto(
        int TotalTools,
        int ActiveUsers,
        int ActiveBookings,
        int OverdueBookings,
        double AvgLoanDays7
    );

    public record TopToolDto(Guid ToolId, string Name, int BookingsCount);
    public record DailyCountDto(DateOnly Date, int Count);

    public record StatsResponse(
        UsageSummaryDto Summary,
        IReadOnlyList<TopToolDto> TopToolsLast30,
        IReadOnlyList<DailyCountDto> BookingsPerDayLast30
    );
}
