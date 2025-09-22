using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models.Bookings;

namespace TooliRent.Infrastructure.Auth
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public bool IsActive { get; set; } = true;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
