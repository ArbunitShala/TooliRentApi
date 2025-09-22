using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.DTOs.Auth
{
    public sealed record LoginRequest(string Email, string Password);
}
