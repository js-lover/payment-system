using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;
using payment_system.Infrastructure.Persistence.Contexts;

namespace payment_system.Infrastructure.Repositories
{
    /// <summary>
    /// Card repository implementation.
    /// Handles database operations for cards.
    /// </summary>
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CardRepository> _logger;

        public CardRepository(AppDbContext context, ILogger<CardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ===== READ Operations =====

        public async Task<Card?> GetByIdAsync(Guid cardId)
        {
            try
            {
                return await _context.Cards
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.Id == cardId && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving card by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            try
            {
                return await _context.Cards
                    .Include(c => c.Account)
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all cards: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Card>> GetAllByAccountIdAsync(Guid accountId)
        {
            try
            {
                return await _context.Cards
                    .Where(c => c.AccountId == accountId && !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving account cards: {ex.Message}");
                throw;
            }
        }

        public async Task<Card?> GetByCardNumberAsync(string cardNumber)
        {
            try
            {
                return await _context.Cards
                    .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving card by number: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Card>> GetActiveCardsByAccountIdAsync(Guid accountId)
        {
            try
            {
                return await _context.Cards
                    .Where(c => c.AccountId == accountId 
                        && c.Status == CardStatus.Active 
                        && !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving active cards: {ex.Message}");
                throw;
            }
        }

        // ===== WRITE Operations =====

        public async Task<Card> CreateAsync(Card card)
        {
            try
            {
                card.CreatedAt = DateTime.UtcNow;
                card.IsDeleted = false;

                _context.Cards.Add(card);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Card created: {card.Id}");
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating card: {ex.Message}");
                throw;
            }
        }

        public async Task<Card> UpdateAsync(Card card)
        {
            try
            {
                card.UpdatedAt = DateTime.UtcNow;
                _context.Cards.Update(card);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Card updated: {card.Id}");
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating card: {ex.Message}");
                throw;
            }
        }

        // ===== DELETE Operations =====

        public async Task<bool> DeleteAsync(Guid cardId)
        {
            try
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
                if (card == null)
                    return false;

                // Soft delete
                card.IsDeleted = true;
                card.UpdatedAt = DateTime.UtcNow;
                _context.Cards.Update(card);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Card deleted: {cardId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting card: {ex.Message}");
                throw;
            }
        }

        // ===== CHECK Operations =====

        public async Task<bool> ExistsAsync(Guid cardId)
        {
            try
            {
                return await _context.Cards
                    .AnyAsync(c => c.Id == cardId && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking card existence: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            try
            {
                return await _context.Cards
                    .AnyAsync(c => c.CardNumber == cardNumber && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking card number existence: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AccountHasCardAsync(Guid accountId, Guid cardId)
        {
            try
            {
                return await _context.Cards
                    .AnyAsync(c => c.AccountId == accountId 
                        && c.Id == cardId 
                        && !c.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking account card: {ex.Message}");
                throw;
            }
        }
    }
}
