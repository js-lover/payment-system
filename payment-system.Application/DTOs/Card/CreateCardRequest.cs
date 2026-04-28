using System;
using System.ComponentModel.DataAnnotations;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Yeni kart oluşturma request'i
    /// </summary>
    public class CreateCardRequest
    {
        public string CardNumber { get; set; } = null!;
        public string CardName { get; set; } = null!;
        public string ExpirationDate { get; set; } = null!;
        public Guid AccountId { get; set; }
    }
}
