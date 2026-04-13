using payment_system.Domain.Enums;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Kart güncelleme request'i
    /// </summary>
    public class UpdateCardRequest
    {
        public string? CardName { get; set; }

        public CardStatus? Status { get; set; }
    }
}
