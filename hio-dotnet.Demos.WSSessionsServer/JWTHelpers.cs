using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hio_dotnet.Demos.WSSessionsServer
{
    public static class JWTHelpers
    {
        public static string GenerateJwtToken(IConfigurationSection jwtSettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "User")
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public static bool CheckToken(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return false;
            }
            var token = authHeader["Bearer ".Length..].Trim();
            if (token.Contains("{") && token.Contains("}"))
            {
                try
                {
                    // parse as json
                    var json = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(token);
                    if (json == null)
                    {
                        return false;
                    }
                    token = json["token"] ?? string.Empty;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            var jwtData = MainDataContext.GetTokenByToken(token);
            if (jwtData == null)
            {
                return false;
            }
            return true;
        }
    }
}