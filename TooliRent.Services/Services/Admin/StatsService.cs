using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Admin;
using TooliRent.Core.Interfaces.Admin;
using TooliRent.Core.Models.Bookings;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Services.Services.Admin
{
    public class StatsService : IStatsService
    {
        private readonly ApplicationDbContext _db;
        public StatsService(ApplicationDbContext db) { _db = db; }

        public async Task<StatsResponse> GetDashboardAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var start30 = now.Date.AddDays(-29); // inkl idag => 30 dagar

            // Sammanfattning
            var totalTools = await _db.Tools.CountAsync(ct);
            var activeUsers = await _db.Users.CountAsync(u => u.IsActive, ct);
            var activeBookings = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.Active, ct);
            var overdue = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.Overdue, ct);

            // Avg lånetid (Completed returnerade senaste 30) – räkna dagarna i minnet
            var completedLast30 = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookingStatus.Completed
                            && b.PickedUpAtUtc != null
                            && b.ReturnedAtUtc != null
                            && b.ReturnedAtUtc >= start30)
                .Select(b => new { Start = b.PickedUpAtUtc!.Value, End = b.ReturnedAtUtc!.Value })
                .ToListAsync(ct);

            var avgLoanDays30 = completedLast30.Count == 0
                ? 0.0
                : completedLast30.Average(x => (x.End - x.Start).TotalDays);

            // Top tools (bokningar som startar inom senaste 30) – projicera anonymt, map i minnet
            var topToolsRaw = await _db.BookingTools
                .AsNoTracking()
                .Where(bt => bt.Booking.StartTime >= start30)
                .GroupBy(bt => new { bt.ToolId, bt.Tool.Name })
                .Select(g => new { g.Key.ToolId, g.Key.Name, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync(ct);

            var topTools = topToolsRaw
                .Select(x => new TopToolDto(x.ToolId, x.Name, x.Count))
                .ToList();

            // Bokningar per dag (start inom senaste 30) – gruppera på DateTime.Date, map till DateOnly i minnet
            var perDayRaw = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.StartTime >= start30)
                .GroupBy(b => b.StartTime.Date) // DateTime
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync(ct);

            var perDay = perDayRaw
                .Select(x => new DailyCountDto(DateOnly.FromDateTime(x.Date), x.Count))
                .ToList();

            var summary = new UsageSummaryDto(
                totalTools,
                activeUsers,
                activeBookings,
                overdue,
                avgLoanDays30
            );

            return new StatsResponse(summary, topTools, perDay);
        }
    }
}
