using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Core.DTOs.Bookings
{
    public class BookingSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public BookingStatus Status { get; set; }
        public int ToolCount { get; set; }
    }
}
