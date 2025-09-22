using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, IEnumerable<string> roles, out DateTime expiresUtc);
    }
}
