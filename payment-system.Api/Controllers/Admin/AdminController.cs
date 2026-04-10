using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Admin;
using payment_system.Application.DTOs.Auth;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Enums;

namespace payment_system.Api.Controllers.Admin
{
    /// <summary>
    /// Admin yönetim controller'ı
    /// Sadece Admin kullanıcıları tarafından erişilebilir
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // ✅ GÜVENLIK: Sadece Admin erişebilir
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Tüm admin kullanıcılarını listele
        /// 
        /// Güvenlik: Sadece Admin kullanıcılar erişebilir
        /// Authorization: [Authorize(Roles = "Admin")]
        /// 
        /// Örnek İstek:
        /// GET /api/admin
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Başarılı Cevap (200):
        /// [
        ///   {
        ///     "id": "550e8400-e29b-41d4-a716-446655440000",
        ///     "email": "admin1@example.com",
        ///     "name": "Yönetici",
        ///     "surname": "Birinci"
        ///   },
        ///   {
        ///     "id": "550e8400-e29b-41d4-a716-446655440001",
        ///     "email": "admin2@example.com",
        ///     "name": "Yönetici",
        ///     "surname": "İkinci"
        ///   }
        /// ]
        /// 
        /// Başarısız Cevap (401):
        /// {
        ///   "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
        ///   "title": "Unauthorized",
        ///   "status": 401,
        ///   "traceId": "..."
        /// }
        /// </summary>
        /// <returns>Tüm admin kullanıcıların listesi</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<AdminsDto>>> GetAllAdmins()
        {
            var result = await _authService.GetAllAdminsAsync();
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Yeni admin kullanıcısı oluştur
        /// 
        /// Güvenlik: Sadece Admin kullanıcılar oluşturabilir
        /// Authorization: [Authorize(Roles = "Admin")]
        /// 
        /// İstek Body:
        /// {
        ///   "email": "newadmin@example.com",
        ///   "password": "SecurePass123!@#",
        ///   "confirmPassword": "SecurePass123!@#",
        ///   "role": 1  // 1 = Admin (Customer = 0, Admin = 1)
        /// }
        /// 
        /// Başarılı Cevap (201):
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "email": "newadmin@example.com",
        ///   "role": 1
        /// }
        /// </summary>
        /// <param name="request">Admin kayıt isteği</param>
        /// <returns>Oluşturulan admin ve JWT token</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<dynamic>> CreateAdmin([FromBody] RegisterRequest request)
        {
            // Admin oluşturma isteğinde role otomatik olarak Admin olmalı
            // Güvenlik açısından request'teki role'ü görmezden geleceğiz
            request.Role = UserRole.Admin;

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetAllAdmins), new { email = request.Email }, result.Data);
        }

        /// <summary>
        /// Admin Dashboard İstatistikleri (İleride eklenecek)
        /// 
        /// Örnek:
        /// - Toplam müşteri sayısı
        /// - Toplam transaction sayısı
        /// - Günlük kazanç
        /// - Son giriş yapan kullanıcılar
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<dynamic> GetDashboard()
        {
            // TODO: Istatistik verilerini döndür
            return Ok(new
            {
                message = "Admin Dashboard - İleride uygulanacak",
                totalCustomers = 0,
                totalTransactions = 0,
                dailyRevenue = 0
            });
        }
    }
}
