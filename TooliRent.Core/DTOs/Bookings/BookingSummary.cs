using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Bookings
{
    public class BookingSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsCancelled { get; set; }
        public int ToolCount { get; set; }
    }
}
