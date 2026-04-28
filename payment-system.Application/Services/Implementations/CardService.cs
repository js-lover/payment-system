using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Card;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;

namespace payment_system.Application.Services.Implementations
{
    /// <summary>
    /// Kart servisi implementasyonu
    /// Business logic ve validation'ı handle eder
    /// </summary>
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CardService> _logger;

        public CardService(
            ICardRepository cardRepository,
            IAccountRepository accountRepository,
            IMapper mapper,
            ILogger<CardService> logger)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ===== READ Operations =====

        public async Task<Result<CardDto>> GetCardByIdAsync(Guid cardId)
        {
            try
            {
                if (cardId == Guid.Empty)
                    return Result<CardDto>.Failure("Geçersiz kart ID'si", 400);

                var card = await _cardRepository.GetByIdAsync(cardId);
                if (card == null)
                    return Result<CardDto>.Failure("Kart bulunamadı", 404);

                var cardDto = _mapper.Map<CardDto>(card);
                _logger.LogInformation($"Kart başarıyla getirildi: {cardId}");

                return Result<CardDto>.Success(cardDto, "Kart başarıyla getirildi", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart getirme hatası: {ex.Message}");
                return Result<CardDto>.Failure("Bir hata oluştu", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetAllCardsAsync()
        {
            try
            {
                var cards = await _cardRepository.GetAllAsync();
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);

                _logger.LogInformation($"Tüm kartlar getirildi: {cards.Count()} adet");
                return Result<IEnumerable<CardDto>>.Success(
                    cardDtos,
                    "Kartlar başarıyla getirildi",
                    200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kartlar getirme hatası: {ex.Message}");
                return Result<IEnumerable<CardDto>>.Failure("Bir hata oluştu", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetCardsByAccountIdAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<IEnumerable<CardDto>>.Failure("Geçersiz hesap ID'si", 400);

                // Hesabın var olup olmadığını kontrol et
                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<IEnumerable<CardDto>>.Failure("Hesap bulunamadı", 404);

                var cards = await _cardRepository.GetAllByAccountIdAsync(accountId);
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);

                _logger.LogInformation($"Hesap {accountId} için {cards.Count()} kart getirildi");
                return Result<IEnumerable<CardDto>>.Success(
                    cardDtos,
                    "Hesabın kartları başarıyla getirildi",
                    200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hesap kartlarını getirme hatası: {ex.Message}");
                return Result<IEnumerable<CardDto>>.Failure("Bir hata oluştu", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetActiveCardsByAccountIdAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<IEnumerable<CardDto>>.Failure("Geçersiz hesap ID'si", 400);

                var cards = await _cardRepository.GetActiveCardsByAccountIdAsync(accountId);
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);

                return Result<IEnumerable<CardDto>>.Success(cardDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Active kartları getirme hatası: {ex.Message}");
                return Result<IEnumerable<CardDto>>.Failure("Bir hata oluştu", 500);
            }
        }

        // ===== WRITE Operations =====

        public async Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request)
        {
            try
            {
                // Input validation
                if (request == null)
                    return Result<CardDto>.Failure("Request verisi boş olamaz", 400);

                // Validate expiration date format (MM/YY)
                if (!IsValidExpirationDate(request.ExpirationDate))
                    return Result<CardDto>.Failure("Geçersiz son kullanma tarihi formatı (MM/YY gerekli)", 400);

                // Check if card number is 16 digits
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.CardNumber, @"^\d{16}$"))
                    return Result<CardDto>.Failure("Kart numarası 16 hane olmalıdır", 400);

                // Check if account exists
                var account = await _accountRepository.GetByIdAsync(request.AccountId);
                if (account == null)
                    return Result<CardDto>.Failure("Hesap bulunamadı", 404);

                // Check card number uniqueness
                if (await _cardRepository.CardNumberExistsAsync(request.CardNumber))
                    return Result<CardDto>.Failure("Bu kart numarası zaten kayıtlı", 400);

                // Create card entity
                var card = new Card
                {
                    Id = Guid.NewGuid(),
                    CardNumber = request.CardNumber,
                    CardName = request.CardName,
                    ExpirationDate = request.ExpirationDate,
                    AccountId = request.AccountId,
                    Status = CardStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    
                };

                // Save to database
                var createdCard = await _cardRepository.CreateAsync(card);

                var cardDto = _mapper.Map<CardDto>(createdCard);
                _logger.LogInformation($"Kart oluşturuldu: {card.Id}");

                return Result<CardDto>.Success(
                    cardDto,
                    "Kart başarıyla oluşturuldu",
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart oluşturma hatası: {ex.Message}");
                return Result<CardDto>.Failure("Kart oluşturulamadı", 500);
            }
        }

        public async Task<Result<CardDto>> UpdateCardAsync(Guid cardId, UpdateCardRequest request)
        {
            try
            {
                if (cardId == Guid.Empty)
                    return Result<CardDto>.Failure("Geçersiz kart ID'si", 400);

                var card = await _cardRepository.GetByIdAsync(cardId);
                if (card == null)
                    return Result<CardDto>.Failure("Kart bulunamadı", 404);

                // Update allowed fields
                if (!string.IsNullOrEmpty(request.CardName))
                    card.CardName = request.CardName;

                if (request.Status.HasValue)
                    card.Status = request.Status.Value;

                card.UpdatedAt = DateTime.UtcNow;

                var updatedCard = await _cardRepository.UpdateAsync(card);
                var cardDto = _mapper.Map<CardDto>(updatedCard);

                _logger.LogInformation($"Kart güncellendi: {cardId}");
                return Result<CardDto>.Success(cardDto, "Kart başarıyla güncellendi", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart güncelleme hatası: {ex.Message}");
                return Result<CardDto>.Failure("Kart güncellenemedi", 500);
            }
        }

        // ===== DELETE Operations =====

        public async Task<Result<bool>> DeleteCardAsync(Guid cardId)
        {
            try
            {
                if (cardId == Guid.Empty)
                    return Result<bool>.Failure("Geçersiz kart ID'si", 400);

                var exists = await _cardRepository.ExistsAsync(cardId);
                if (!exists)
                    return Result<bool>.Failure("Kart bulunamadı", 404);

                var result = await _cardRepository.DeleteAsync(cardId);
                if (!result)
                    return Result<bool>.Failure("Kart silinemedi", 500);

                _logger.LogInformation($"Kart silindi: {cardId}");
                return Result<bool>.Success(true, "Kart başarıyla silindi", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kart silme hatası: {ex.Message}");
                return Result<bool>.Failure("Kart silinemedi", 500);
            }
        }

        // ===== Helper Methods =====

        /// <summary>
        /// Son kullanma tarihinin geçerli olup olmadığını kontrol et (MM/YY formatı)
        /// </summary>
        private bool IsValidExpirationDate(string expirationDate)
        {
            try
            {
                // Format: MM/YY (örn: 12/25)
                if (!System.Text.RegularExpressions.Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])/\d{2}$"))
                    return false;

                var parts = expirationDate.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]);

                // Current date
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year % 100; // Last 2 digits

                // Card expires at the end of the expiration month
                // So it's valid until the last day of the month
                if (year < currentYear)
                    return false;

                if (year == currentYear && month < currentMonth)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
