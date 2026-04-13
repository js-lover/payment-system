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
    /// Kart repository implementasyonu
    /// Database operasyonlarını handle eder
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
                _logger.LogError($"Kartı ID'ye göre getirme hatası: {ex.Message}");
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
                _logger.LogError($"Tüm kartları getirme hatası: {ex.Message}");
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
                _logger.LogError($"Hesap kartlarını getirme hatası: {ex.Message}");
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
                _logger.LogError($"Kart numarasına göre getirme hatası: {ex.Message}");
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
                _logger.LogError($"Active kartları getirme hatası: {ex.Message}");
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

                _logger.LogInformation($"Kart oluşturuldu: {card.Id}");
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart oluşturma hatası: {ex.Message}");
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

                _logger.LogInformation($"Kart güncellendi: {card.Id}");
                return card;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart güncelleme hatası: {ex.Message}");
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

                _logger.LogInformation($"Kart silindi: {cardId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart silme hatası: {ex.Message}");
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
                _logger.LogError($"Kart varlığı kontrol hatası: {ex.Message}");
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
                _logger.LogError($"Kart numarası varlığı kontrol hatası: {ex.Message}");
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
                _logger.LogError($"Hesap kartı kontrol hatası: {ex.Message}");
                throw;
            }
        }
    }
}
