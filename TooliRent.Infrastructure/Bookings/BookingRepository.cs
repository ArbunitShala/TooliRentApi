using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces.Bookings;
using TooliRent.Core.Models.Bookings;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Infrastructure.Bookings
{
    public class BookingRepository :  IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db) => _db = db;

        public async Task<bool> HasOverlapAsync(Guid toolId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
        {
            // Kolla överlapp mot icke-avbokade bokningar som innehåller verktyget
            return await _db.Set<Booking>()
                .AsNoTracking()
                .Where(b => b.Status != BookingStatus.Cancelled)
                .Where(b => !(endUtc <= b.StartTime || startUtc >= b.EndTime)) // overlap
                .AnyAsync(b => b.BookingTools.Any(bt => bt.ToolId == toolId), ct);
        }

        public async Task<Booking> AddAsync(Booking booking, CancellationToken ct = default)
        {
            _db.Set<Booking>().Add(booking);
            await _db.SaveChangesAsync(ct);
            return booking;
        }

        public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Set<Booking>()
                .Include(b => b.BookingTools)
                    .ThenInclude(bt => bt.Tool) // kräver navigation i BookingTool
                .FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<List<Booking>> GetByUserAsync(Guid userId, CancellationToken ct = default)
            => _db.Set<Booking>()
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartTime)
                .Include(b => b.BookingTools)
                .ToListAsync(ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
