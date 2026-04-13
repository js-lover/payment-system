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

                // Validate expiration date
                if (request.ExpirationDate <= DateTime.Now)
                    return Result<CardDto>.Failure("Son kullanma tarihi geçmiş olamaz", 400);


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
                    CVC = EncryptCVC(request.CVC), // Encrypt CVC
                    AccountId = request.AccountId,
                    Status = CardStatus.WaitingForApproval,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
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
        /// CVC'yi şifrele
        /// </summary>
        private string EncryptCVC(string cvc)
        {
            try
            {
                // Production'da AES encryption kullan
                // Şimdilik Base64 (PLACEHOLDER)
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cvc));
            }
            catch
            {
                return cvc;
            }
        }

        /// <summary>
        /// CVC'yi deşifrele
        /// </summary>
        private string DecryptCVC(string encryptedCvc)
        {
            try
            {
                // Production'da AES decryption kullan
                // Şimdilik Base64 (PLACEHOLDER)
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedCvc));
            }
            catch
            {
                return encryptedCvc;
            }
        }
    }
}
