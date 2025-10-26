using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiStart.Services
{
    public interface IJwtTokenService
  {
        string GenerateToken(int userId, string email);
    ClaimsPrincipal? ValidateToken(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string email)
  {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "your-secret-key-minimum-32-characters-long"));
     var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

   var claims = new[]
         {
   new Claim("userId", userId.ToString()),
        new Claim(ClaimTypes.Email, email),
        };

      var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"] ?? "ApiStart",
             audience: _configuration["Jwt:Audience"] ?? "ApiStartUsers",
        claims: claims,
             expires: DateTime.UtcNow.AddHours(1),
   signingCredentials: credentials
            );

   return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
      {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "your-secret-key-minimum-32-characters-long"));
           var tokenHandler = new JwtSecurityTokenHandler();

       var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
       {
          ValidateIssuerSigningKey = true,
 IssuerSigningKey = key,
        ValidateIssuer = false,
          ValidateAudience = false,
     ClockSkew = TimeSpan.Zero
     }, out SecurityToken validatedToken);

            return principal;
            }
     catch
            {
         return null;
    }
        }
    }
}
