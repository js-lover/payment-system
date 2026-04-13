using System;
using payment_system.Domain.Enums;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Kart bilgilerini API response'da döndürmek için kullanılan DTO
    /// Sensitive bilgiler (CVC) burada bulunmaz
    /// CardNumber masked format'tedir
    /// </summary>
    public class CardDto
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Masked card number: "1234 **** **** 6789"
        /// </summary>
        public string CardNumber { get; set; } = null!;
        
        public string CardName { get; set; } = null!;
        
        public DateTime ExpirationDate { get; set; }
        
        public Guid AccountId { get; set; }
        
        public CardStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
