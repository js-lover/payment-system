using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Card repository interface.
    /// Defines database operations for cards.
    /// </summary>
    public interface ICardRepository
    {
        // ===== READ Operations =====

        /// <summary>
        /// Get card by ID.
        /// </summary>
        Task<Card?> GetByIdAsync(Guid cardId);

        /// <summary>
        /// Get all cards.
        /// </summary>
        Task<IEnumerable<Card>> GetAllAsync();

        /// <summary>
        /// Get all cards for an account.
        /// </summary>
        Task<IEnumerable<Card>> GetAllByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Get card by card number (unique).
        /// </summary>
        Task<Card?> GetByCardNumberAsync(string cardNumber);

        /// <summary>
        /// Get active cards for an account.
        /// </summary>
        Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId);

        // ===== WRITE Operations =====

        /// <summary>
        /// Create a new card.
        /// </summary>
        Task<Card> CreateAsync(Card card);

        /// <summary>
        /// Update a card.
        /// </summary>
        Task<Card> UpdateAsync(Card card);

        // ===== DELETE Operations =====

        /// <summary>
        /// Delete a card.
        /// </summary>
        Task<bool> DeleteAsync(Guid cardId);

        // ===== CHECK Operations =====

        /// <summary>
        /// Check if a card exists.
        /// </summary>
        Task<bool> ExistsAsync(Guid cardId);

        /// <summary>
        /// Check if a card number already exists.
        /// </summary>
        Task<bool> CardNumberExistsAsync(string cardNumber);

        /// <summary>
        /// Check if account has a card.
        /// </summary>
        Task<bool> AccountHasCardAsync(Guid accountId, Guid cardId);
    }
}
