using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces.Auth;

namespace TooliRent.Infrastructure.Auth
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;
        public JwtTokenGenerator(IConfiguration config) => _config = config;

        public string GenerateToken(Guid userId, string email, IEnumerable<string> roles, out DateTime expiresUtc)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, email)
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expireMinutes = int.TryParse(jwt["ExpireMinutes"], out var m) ? m : 60;
            expiresUtc = DateTime.UtcNow.AddMinutes(expireMinutes);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expiresUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
