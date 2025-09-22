using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Core.Models.Catalog
{
    public enum ToolStatus { Available, Booked, CheckedOut, Maintenance }

    public class Tool
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ToolStatus Status { get; set; } = ToolStatus.Available;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public ICollection<BookingTool> BookingTools { get; set; } = new List<BookingTool>();
    }
}
