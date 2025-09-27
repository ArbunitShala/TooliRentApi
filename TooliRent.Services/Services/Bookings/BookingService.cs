using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;
using TooliRent.Core.Interfaces.Bookings;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Services.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookingRequest> _validator;

        public BookingService(IBookingRepository repo, IMapper mapper, IValidator<CreateBookingRequest> validator)
        {
            _repo = repo;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BookingDetailsDto> CreateAsync(Guid userId, CreateBookingRequest req, CancellationToken ct = default)
        {
            // 1) DTO-validering
            var v = await _validator.ValidateAsync(req, ct);
            if (!v.IsValid) throw new ValidationException(v.Errors);

            // 2) ingen överlappning för något valt verktyg
            var conflicts = new List<Guid>();
            foreach (var toolId in req.ToolIds)
            {
                var overlap = await _repo.HasOverlapAsync(toolId, req.StartUtc, req.EndUtc, ct);
                if (overlap) conflicts.Add(toolId);
            }
            if (conflicts.Count > 0)
                throw new InvalidOperationException($"Följande verktyg är redan bokade under denna period: {string.Join(", ", conflicts)}");

            // 3) Skapa bokning
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartTime = req.StartUtc,
                EndTime = req.EndUtc,
                Status = BookingStatus.Active,
                BookingTools = req.ToolIds.Select(id => new BookingTool { ToolId = id }).ToList()
            };

            booking = await _repo.AddAsync(booking, ct);

            // 4) Hämta inkl. Tool för att kunna mappa Tool-namn
            var created = await _repo.GetByIdAsync(booking.Id, ct)!;
            return _mapper.Map<BookingDetailsDto>(created);
        }

        public async Task<IReadOnlyList<BookingSummaryDto>> GetMyBookingsAsync(Guid userId, CancellationToken ct = default)
        {
            var list = await _repo.GetByUserAsync(userId, ct);
            return list.Select(_mapper.Map<BookingSummaryDto>).ToList();
        }

        public async Task<BookingDetailsDto?> GetByIdAsync(Guid requesterId, Guid bookingId, bool isAdmin, CancellationToken ct = default)
        {
            var b = await _repo.GetByIdAsync(bookingId, ct);
            if (b is null) return null;
            if (!isAdmin && b.UserId != requesterId) return null;
            return _mapper.Map<BookingDetailsDto>(b);
        }

        public async Task<bool> CancelAsync(Guid requesterId, Guid bookingId, bool isAdmin, CancellationToken ct = default)
        {
            var b = await _repo.GetByIdAsync(bookingId, ct);
            if (b is null) return false;
            if (!isAdmin && b.UserId != requesterId) return false;

            if (b.Status == BookingStatus.Cancelled) return true; // redan avbokad

            b.Status = BookingStatus.Cancelled;
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
