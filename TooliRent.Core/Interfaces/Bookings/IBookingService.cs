using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;

namespace TooliRent.Core.Interfaces.Bookings
{
    public interface IBookingService
    {
        Task<BookingDetailsDto> CreateAsync(Guid userId, CreateBookingRequest req, CancellationToken ct = default);
        Task<IReadOnlyList<BookingSummaryDto>> GetMyBookingsAsync(Guid userId, CancellationToken ct = default);
        Task<BookingDetailsDto?> GetByIdAsync(Guid requesterId, Guid bookingId, bool isAdmin, CancellationToken ct = default);
        Task<bool> CancelAsync(Guid requesterId, Guid bookingId, bool isAdmin, CancellationToken ct = default);

        Task<BookingDetailsDto?> PickupAsync(Guid requesterId, Guid bookingId, bool isAdmin, DateTime? whenUtc, CancellationToken ct = default);
        Task<BookingDetailsDto?> ReturnAsync(Guid requesterId, Guid bookingId, bool isAdmin, DateTime? whenUtc, CancellationToken ct = default);
    }
}
