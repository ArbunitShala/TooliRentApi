using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        // lista av Tool-Id som ska bokas
        public List<Guid> ToolIds { get; set; } = new();
    }
}
