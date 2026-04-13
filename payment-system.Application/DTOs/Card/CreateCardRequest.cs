using System;
using System.ComponentModel.DataAnnotations;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Yeni kart oluşturma request'i
    /// </summary>
    public class CreateCardRequest
    {
        [Required(ErrorMessage = "Kart numarası zorunludur")]
        [RegularExpression(@"^\d{13,19}$", 
            ErrorMessage = "Geçerli bir kart numarası giriniz (13-19 hane)")]
        public string CardNumber { get; set; } = null!;

        [Required(ErrorMessage = "Kart adı zorunludur")]
        [StringLength(100, MinimumLength = 1, 
            ErrorMessage = "Kart adı 1-100 karakter arasında olmalıdır")]
        public string CardName { get; set; } = null!;

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur")]
        [RegularExpression(@"^(0[1-9]|1[0-2])/\d{2}$", 
            ErrorMessage = "Son kullanma tarihi MM/YY formatında olmalıdır (örn: 12/25)")]
        public string ExpirationDate { get; set; } = null!;

        [Required(ErrorMessage = "CVC zorunludur")]
        [RegularExpression(@"^\d{3,4}$", 
            ErrorMessage = "CVC 3-4 haneli bir sayı olmalıdır")]
        public string CVC { get; set; } = null!;

        [Required(ErrorMessage = "Hesap ID zorunludur")]
        public Guid AccountId { get; set; }
    }
}
