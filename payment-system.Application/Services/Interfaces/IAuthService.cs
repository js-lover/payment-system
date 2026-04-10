//kullanıcının giriş talebini işleme alır, kimliği doğrular ve bir JWT teslim eder

using payment_system.Application.Common;
using payment_system.Application.DTOs.Admin;
using payment_system.Application.DTOs.Auth;

namespace payment_system.Application.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcının giriş talebini işleme alır, kimliği doğrular ve bir JWT teslim eder.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task <Result<AuthResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Yeni kullanıcı kaydı (Register)
        /// 
        /// İş Akışı:
        /// 1. Email doğrulaması (zaten kayıtlı mı?)
        /// 2. Şifre doğrulaması (güç ve uyuşma)
        /// 3. User oluştur (Identity) - Email, şifre, rol
        /// 4. Eğer Customer ise, müşteri profili oluştur
        /// 5. JWT token döndür
        /// </summary>
        /// <param name="request">Kayıt isteği (Email, şifre, rol, profil bilgileri)</param>
        /// <returns>JWT token ve kullanıcı bilgileri veya hata</returns>
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);


        /// <summary>
        /// Tüm admin kullanıcılarını getirir.
        /// </summary>
        /// <returns></returns>
        Task<Result<List<AdminsDto>>> GetAllAdminsAsync();
    }
}