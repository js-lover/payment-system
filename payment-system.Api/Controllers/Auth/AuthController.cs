using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Auth;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Common;

namespace payment_system.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Kullanıcı giriş işlemini gerçekleştirir ve JWT döner.
        /// </summary>
        /// <param name="request">Email ve Şifre bilgilerini içeren istek</param>
        /// <returns>Başarılıysa Token, başarısızsa hata mesajı</returns>
        [AllowAnonymous] // Herkes erişebilir, çünkü henüz giriş yapılmadı
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. İş mantığını (Service) tetikle
            var result = await _authService.LoginAsync(request);

            // 2. Sonucu kontrol et
            if (!result.IsSuccess)
            {
                // Güvenlik gereği 401 Unauthorized dönüyoruz
                return Unauthorized(new { message = result.Message });
            }

            // 3. Başarılıysa 200 OK ve Token bilgilerini dön
            return Ok(result.Data);
        }

        /// <summary>
        /// Yeni kullanıcı kaydı (Register)
        /// 
        /// "User-First" Mimarisi:
        /// 1. Rol seçerek kullanıcı oluştur (Customer, Admin, Staff)
        /// 2. Customer ise profil bilgileri ekle
        /// 3. JWT token döndür
        /// 
        /// Örnek Customer Kaydı:
        /// {
        ///   "email": "customer@example.com",
        ///   "password": "Pass123!",
        ///   "confirmPassword": "Pass123!",
        ///   "role": 0,  // 0 = Customer, 1 = Admin
        ///   "name": "Ahmet",
        ///   "surname": "Yılmaz",
        ///   "nationalId": "12345678901",
        ///   "phoneNumber": "+905551234567",
        ///   "dateOfBirth": "1990-05-15"
        /// }
        /// 
        /// Örnek Admin Kaydı (Profil bilgileri isteğe bağlı):
        /// {
        ///   "email": "admin@example.com",
        ///   "password": "Pass123!",
        ///   "confirmPassword": "Pass123!",
        ///   "role": 1  // 1 = Admin (profil oluşturulmaz)
        /// }
        /// </summary>
        /// <param name="request">Kayıt isteği</param>
        /// <returns>JWT token veya hata mesajı</returns>
        [AllowAnonymous] // Kayıt açık olmalı
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. İş mantığını (Service) tetikle
            var result = await _authService.RegisterAsync(request);

            // 2. Sonucu kontrol et
            if (!result.IsSuccess)
            {
                // Güvenlik gereği 400 Bad Request dönüyoruz
                return BadRequest(new { message = result.Message });
            }

            // 3. Başarılıysa 201 Created ve Token bilgilerini dön
            return CreatedAtAction(nameof(Login), new { email = request.Email }, result.Data);
        }


    }
}