using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using payment_system.Application.Services.Interfaces;
using System.Text;
using payment_system.Domain.Entities;

namespace payment_system.Infrastructure.Security
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(User user)
        {
            // 1. Ayarları güvenli bir şekilde çekelim
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKeyString = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing!");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Claims (Kimlik Bilgileri)
            // ASP.NET Core Authorization filter'larının (Authorize attribute) 
            // doğru çalışması için ClaimTypes kullanılması önerilir.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()), // Enum'ı string'e çeviriyoruz
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 3. Süreyi appsettings'ten çekelim, yoksa varsayılan 60 dk yapalım
            var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

            // 4. Token Tanımlayıcı
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes), // Hata buradaydı, düzeltildi
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = creds
            };

            // 5. Üretim
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}