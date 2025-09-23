using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Auth
{
    public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);
}
