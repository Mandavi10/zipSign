using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace JWT_Verification
{
    public class VerifyToken
    {
        private static ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("a1b2c3d4e5f6g7h8i9j0kA1B2C3D4E5F6G7H8I9J0");
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true, // Set to true if you want to validate issuer
                ValidIssuer = "Test", // Specify the valid issuer
                ValidateAudience = true, // Set to true if you want to validate audience
                ValidAudience = "Demo", // DemoSpecify the valid audience
                ValidateLifetime = true, // Set to true to validate token expiration
                ClockSkew = TimeSpan.Zero // Adjust the clock skew if needed
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Errors($"Security token expired: {ex.Message}");
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GetUserIdFromToken(string Token)
        {
            try
            {
                if (Token != null)
                {
                    var principal = ValidateJwtToken(Token);
                    if (principal != null)
                    {
                        var userIdClaim = principal.FindFirst(ClaimTypes.Name);
                        return userIdClaim?.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }
        private static string Errors(string errors)
        {
            return errors;
        }
    }
}
