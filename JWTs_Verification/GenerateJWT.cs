using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTs_Verification
{
    public class GenerateJWT
    {
        public static string GenerateJwtToken(string secretKey, string issuer, string audience, string userId)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] { new Claim(ClaimTypes.Name, userId) },
                expires: DateTime.UtcNow.AddMinutes(2), // Adjust the token expiration as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
