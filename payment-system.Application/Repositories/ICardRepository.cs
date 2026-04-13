using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Kart repository arayüzü
    /// Veritabanı operasyonlarını tanımlar
    /// </summary>
    public interface ICardRepository
    {
        // ===== READ Operations =====

        /// <summary>
        /// ID'ye göre kartı getir
        /// </summary>
        Task<Card?> GetByIdAsync(Guid cardId);

        /// <summary>
        /// Tüm kartları getir
        /// </summary>
        Task<IEnumerable<Card>> GetAllAsync();

        /// <summary>
        /// Account'a ait tüm kartları getir
        /// </summary>
        Task<IEnumerable<Card>> GetAllByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Kart numarasına göre kartı getir (unique olduğu için)
        /// </summary>
        Task<Card?> GetByCardNumberAsync(string cardNumber);

        /// <summary>
        /// Active kartları getir (belirli bir account'a ait)
        /// </summary>
        Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId);

        // ===== WRITE Operations =====

        /// <summary>
        /// Yeni kart oluştur
        /// </summary>
        Task<Card> CreateAsync(Card card);

        /// <summary>
        /// Kartı güncelle
        /// </summary>
        Task<Card> UpdateAsync(Card card);

        // ===== DELETE Operations =====

        /// <summary>
        /// Kartı sil
        /// </summary>
        Task<bool> DeleteAsync(Guid cardId);

        // ===== CHECK Operations =====

        /// <summary>
        /// Kartın var olup olmadığını kontrol et
        /// </summary>
        Task<bool> ExistsAsync(Guid cardId);

        /// <summary>
        /// Kart numarasının zaten var olup olmadığını kontrol et
        /// </summary>
        Task<bool> CardNumberExistsAsync(string cardNumber);

        /// <summary>
        /// Account'ın kartı olup olmadığını kontrol et
        /// </summary>
        Task<bool> AccountHasCardAsync(Guid accountId, Guid cardId);
    }
}
