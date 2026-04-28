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
        public string CardNumber { get; set; } = null!;
        public string CardName { get; set; } = null!;
        public string ExpirationDate { get; set; } = null!;
        public Guid AccountId { get; set; }
        public CardStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
