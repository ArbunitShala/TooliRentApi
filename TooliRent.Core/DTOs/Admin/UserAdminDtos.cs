using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Admin
{
    public class UserAdminDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public record SetUserActiveRequest(bool IsActive);
    public record SetUserRoleRequest(string Role);
}
