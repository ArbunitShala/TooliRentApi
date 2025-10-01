using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.DTOs.Bookings;
using TooliRent.Core.Interfaces.Bookings;
using TooliRent.Core.Models.Bookings;
using TooliRent.Infrastructure.Data;

namespace TooliRent.Services.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IBookingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookingRequest> _validator;

        public BookingService(IBookingRepository repo, IMapper mapper, IValidator<CreateBookingRequest> validator,
            ApplicationDbContext db)
        {
            _repo = repo;
            _mapper = mapper;
            _validator = validator;
            _db = db;
        }

        public async Task<BookingDetailsDto> CreateAsync(Guid userId, CreateBookingRequest req, CancellationToken ct = default)
        {
            // 1) DTO-validering (du har redan FluentValidation)
            var v = await _validator.ValidateAsync(req, ct);
            if (!v.IsValid) throw new ValidationException(v.Errors);

            // 2) Normalisera & sanity-check
            var start = DateTime.SpecifyKind(req.StartUtc.UtcDateTime, DateTimeKind.Utc);
            var end = DateTime.SpecifyKind(req.EndUtc.UtcDateTime, DateTimeKind.Utc);
            if (end <= start) throw new InvalidOperationException("EndUtc måste vara efter StartUtc.");

            // 3) ToolIds: ta bort tomma/dubletter
            var toolIds = (req.ToolIds ?? new List<Guid>()).Where(id => id != Guid.Empty).Distinct().ToList();
            if (toolIds.Count == 0) throw new InvalidOperationException("Minst ett verktyg måste väljas.");

            // 4) verifiera att alla verktyg finns i DB annars blir det FK-krasch
            var existing = await _db.Tools.Where(t => toolIds.Contains(t.Id))
                                          .Select(t => t.Id)
                                          .ToListAsync(ct);
            var missing = toolIds.Except(existing).ToList();
            if (missing.Count > 0)
                throw new KeyNotFoundException($"Okända verktygs-ID:n: {string.Join(", ", missing)}");

            // 5) Överlappskontroll (du hade detta – nu kör vi på de rensade IDs)
            var conflicts = new List<Guid>();
            foreach (var toolId in toolIds)
            {
                var overlap = await _repo.HasOverlapAsync(toolId, start, end, ct);
                if (overlap) conflicts.Add(toolId);
            }
            if (conflicts.Count > 0)
                throw new InvalidOperationException($"Följande verktyg är redan bokade under denna period: {string.Join(", ", conflicts)}");

            // 6) Skapa bokningen – VIKTIGT: sätt ENDAST ToolId på join-raderna
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartTime = start,
                EndTime = end,
                Status = BookingStatus.Active,
                BookingTools = toolIds.Select(id => new BookingTool { ToolId = id }).ToList()
            };

            booking = await _repo.AddAsync(booking, ct);

            // 7) Läs tillbaka inkl. navigationer för mappning
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

        // PICKUP ----
        public async Task<BookingDetailsDto?> PickupAsync(Guid requesterId, Guid bookingId, bool isAdmin, DateTime? whenUtc, CancellationToken ct = default)
        {
            var b = await _repo.GetByIdAsync(bookingId, ct);
            if (b is null) return null;
            if (!isAdmin && b.UserId != requesterId) return null;
            if (b.Status == BookingStatus.Cancelled) throw new InvalidOperationException("Bokningen är avbokad.");
            if (b.PickedUpAtUtc != null) return _mapper.Map<BookingDetailsDto>(b); // redan hämtad, idempotent

            var t = whenUtc ?? DateTime.UtcNow;

            // enkel regel, hämta inom bokningsfönstret
            if (t < b.StartTime) throw new InvalidOperationException("Kan inte hämta innan starttid.");
            if (t > b.EndTime) throw new InvalidOperationException("Kan inte hämta efter sluttid.");

            b.PickedUpAtUtc = t;
            // status förblir Active tills retur
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<BookingDetailsDto>(b);
        }

        // RETURN ----
        public async Task<BookingDetailsDto?> ReturnAsync(Guid requesterId, Guid bookingId, bool isAdmin, DateTime? whenUtc, CancellationToken ct = default)
        {
            var b = await _repo.GetByIdAsync(bookingId, ct);
            if (b is null) return null;
            if (!isAdmin && b.UserId != requesterId) return null;
            if (b.Status == BookingStatus.Cancelled) throw new InvalidOperationException("Bokningen är avbokad.");
            if (b.ReturnedAtUtc != null) return _mapper.Map<BookingDetailsDto>(b); // redan återlämnad, idempotent

            if (b.PickedUpAtUtc == null) throw new InvalidOperationException("Kan inte återlämna en bokning som inte hämtats.");

            var t = whenUtc ?? DateTime.UtcNow;
            if (t < b.PickedUpAtUtc.Value) throw new InvalidOperationException("Återlämningstid kan inte vara före hämtning.");

            b.ReturnedAtUtc = t;

            // Sätt status beroende på sen retur
            b.Status = (t > b.EndTime) ? BookingStatus.Overdue : BookingStatus.Completed;

            await _repo.SaveChangesAsync(ct);
            return _mapper.Map<BookingDetailsDto>(b);
        }
    }
}
