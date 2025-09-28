using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Models.Bookings
{
    public enum BookingStatus { Active, Cancelled, Completed, Overdue }

    public class Booking
    {
        public Guid Id { get; set; }

        // Koppling till användare (ApplicationUser finns i Infrastructure)
        public Guid UserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // tidsstämplar för hämta/lämna
        public DateTime? PickedUpAtUtc { get; set; }
        public DateTime? ReturnedAtUtc { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Active;

        public ICollection<BookingTool> BookingTools { get; set; } = new List<BookingTool>();
    }
}
