using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Core.Interfaces.Bookings
{
    public interface IBookingRepository
    {
        Task<bool> HasOverlapAsync(Guid toolId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default);

        Task<Booking> AddAsync(Booking booking, CancellationToken ct = default);

        Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<List<Booking>> GetByUserAsync(Guid userId, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
