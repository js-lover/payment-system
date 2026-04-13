using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Card;

namespace payment_system.Application.Services.Interfaces
{
    /// <summary>
    /// Kart servisi arayüzü
    /// Business logic'i tanımlar
    /// </summary>
    public interface ICardService
    {
        // ===== READ Operations =====

        /// <summary>
        /// ID'ye göre kartı getir
        /// </summary>
        Task<Result<CardDto>> GetCardByIdAsync(Guid cardId);

        /// <summary>
        /// Tüm kartları getir
        /// </summary>
        Task<Result<IEnumerable<CardDto>>> GetAllCardsAsync();

        /// <summary>
        /// Account'a ait tüm kartları getir
        /// </summary>
        Task<Result<IEnumerable<CardDto>>> GetCardsByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Active kartları getir
        /// </summary>
        Task<Result<IEnumerable<CardDto>>> GetActiveCardsByAccountIdAsync(Guid accountId);

        // ===== WRITE Operations =====

        /// <summary>
        /// Yeni kart oluştur
        /// </summary>
        Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request);

        /// <summary>
        /// Kartı güncelle
        /// </summary>
        Task<Result<CardDto>> UpdateCardAsync(Guid cardId, UpdateCardRequest request);

        // ===== DELETE Operations =====

        /// <summary>
        /// Kartı sil
        /// </summary>
        Task<Result<bool>> DeleteCardAsync(Guid cardId);
    }
}
