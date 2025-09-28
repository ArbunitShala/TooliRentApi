using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Core.DTOs.Bookings
{
    public class BookingDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public DateTime? PickedUpAtUtc { get; set; }
        public DateTime? ReturnedAtUtc { get; set; }
        public BookingStatus Status { get; set; }

        public List<ToolItem> Tools { get; set; } = new();

        public record ToolItem(Guid Id, string Name);
    }
}
