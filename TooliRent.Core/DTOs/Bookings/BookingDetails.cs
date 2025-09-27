using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Bookings
{
    public class BookingDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsCancelled { get; set; }

        public List<ToolItem> Tools { get; set; } = new();

        public record ToolItem(Guid Id, string Name);
    }
}
