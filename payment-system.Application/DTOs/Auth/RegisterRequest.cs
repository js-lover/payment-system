using payment_system.Domain.Enums;

namespace payment_system.Application.DTOs.Auth
{
    /// <summary>
    /// Yeni kullanıcı kayıt isteği
    /// 
    /// İş Akışı:
    /// 1. Email ve şifre ile kullanıcı oluştur (Identity - User tablosu)
    /// 2. Rol ata (Customer, Admin, Staff vb)
    /// 3. Eğer Customer ise, Customer profili oluştur (1:0 ilişki)
    /// 4. JWT token döndür
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Email adresi (unique, control center)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Şifre (BCrypt ile hash'lenecek)
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Şifre doğrulama (aynı olmalı)
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcı rolü (Customer, Admin, Staff, Moderator vb)
        /// Default: Customer
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Customer;

        // ===== CUSTOMER PROFİLİ (Sadece Customer role'ü için gerekli) =====

        /// <summary>
        /// Ad (Sadece Customer için)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Soyad (Sadece Customer için)
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// TC Kimlik Numarası (Sadece Customer için)
        /// </summary>
        public string? NationalId { get; set; }

        /// <summary>
        /// Telefon Numarası (Sadece Customer için)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Doğum Tarihi (Sadece Customer için)
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
    }
}
