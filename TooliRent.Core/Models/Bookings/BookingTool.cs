using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Catalog;

namespace TooliRent.Core.Models.Bookings
{
    public class BookingTool
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = default!;

        public Guid ToolId { get; set; }
        public Tool Tool { get; set; } = default!;

        public bool CheckedOut { get; set; }
        public DateTime? CheckedOutAt { get; set; }

        public bool Returned { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
