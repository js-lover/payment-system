# 💻 Kart Yönetimi - Kod Örnekleri & Başlangıç Rehberi

## 1. DTOs (Data Transfer Objects)

### CardDto.cs
```csharp
// Dosya: payment-system.Application/DTOs/Card/CardDto.cs

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
        
        /// <summary>
        /// Masked card number: \"1234 **** **** 6789\"
        /// </summary>
        public string CardNumber { get; set; } = null!;
        
        public string CardName { get; set; } = null!;
        
        public DateTime ExpirationDate { get; set; }
        
        public Guid AccountId { get; set; }
        
        public CardStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
```

### CreateCardRequest.cs
```csharp
// Dosya: payment-system.Application/DTOs/Card/CreateCardRequest.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Yeni kart oluşturma request'i
    /// </summary>
    public class CreateCardRequest
    {
        [Required(ErrorMessage = \"Kart numarası zorunludur\")]
        [RegularExpression(@\"^\\d{13,19}$\", 
            ErrorMessage = \"Geçerli bir kart numarası giriniz (13-19 hane)\")]
        public string CardNumber { get; set; } = null!;

        [Required(ErrorMessage = \"Kart adı zorunludur\")]
        [StringLength(100, MinimumLength = 1, 
            ErrorMessage = \"Kart adı 1-100 karakter arasında olmalıdır\")]
        public string CardName { get; set; } = null!;

        [Required(ErrorMessage = \"Son kullanma tarihi zorunludur\")]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = \"CVC zorunludur\")]
        [RegularExpression(@\"^\\d{3,4}$\", 
            ErrorMessage = \"CVC 3-4 haneli bir sayı olmalıdır\")]
        public string CVC { get; set; } = null!;

        [Required(ErrorMessage = \"Hesap ID zorunludur\")]
        public Guid AccountId { get; set; }
    }
}
```

### UpdateCardRequest.cs
```csharp
// Dosya: payment-system.Application/DTOs/Card/UpdateCardRequest.cs

using payment_system.Domain.Enums;

namespace payment_system.Application.DTOs.Card
{
    /// <summary>
    /// Kart güncelleme request'i
    /// </summary>
    public class UpdateCardRequest
    {
        [StringLength(100, MinimumLength = 1)]
        public string? CardName { get; set; }

        public CardStatus? Status { get; set; }
    }
}
```

---

## 2. Repository Interface

### ICardRepository.cs
```csharp
// Dosya: payment-system.Application/Repositories/ICardRepository.cs

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

        /// <summary>
        /// Süresi dolmak üzere olan kartları getir (30 gün içinde)
        /// </summary>
        Task<IEnumerable<Card>> GetExpiringCardsAsync(int daysThreshold = 30);

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
```

---

## 3. Service Interface

### ICardService.cs
```csharp
// Dosya: payment-system.Application/Services/Interfaces/ICardService.cs

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
```

---

## 4. Service Implementation

### CardService.cs
```csharp
// Dosya: payment-system.Application/Services/Implementations/CardService.cs

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
                    return Result<CardDto>.Failure(\"Geçersiz kart ID'si\", 400);

                var card = await _cardRepository.GetByIdAsync(cardId);
                if (card == null)
                    return Result<CardDto>.Failure(\"Kart bulunamadı\", 404);

                var cardDto = _mapper.Map<CardDto>(card);
                _logger.LogInformation($\"Kart başarıyla getirildi: {cardId}\");
                
                return Result<CardDto>.Success(cardDto, \"Kart başarıyla getirildi\", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Kart getirme hatası: {ex.Message}\");
                return Result<CardDto>.Failure(\"Bir hata oluştu\", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetAllCardsAsync()
        {
            try
            {
                var cards = await _cardRepository.GetAllAsync();
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);
                
                _logger.LogInformation($\"Tüm kartlar getirildi: {cards.Count()} adet\");
                return Result<IEnumerable<CardDto>>.Success(
                    cardDtos, 
                    \"Kartlar başarıyla getirildi\", 
                    200);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Kartlar getirme hatası: {ex.Message}\");
                return Result<IEnumerable<CardDto>>.Failure(\"Bir hata oluştu\", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetCardsByAccountIdAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<IEnumerable<CardDto>>.Failure(\"Geçersiz hesap ID'si\", 400);

                // Hesabın var olup olmadığını kontrol et
                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<IEnumerable<CardDto>>.Failure(\"Hesap bulunamadı\", 404);

                var cards = await _cardRepository.GetAllByAccountIdAsync(accountId);
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);
                
                _logger.LogInformation($\"Hesap {accountId} için {cards.Count()} kart getirildi\");
                return Result<IEnumerable<CardDto>>.Success(
                    cardDtos, 
                    \"Hesabın kartları başarıyla getirildi\", 
                    200);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Hesap kartlarını getirme hatası: {ex.Message}\");
                return Result<IEnumerable<CardDto>>.Failure(\"Bir hata oluştu\", 500);
            }
        }

        public async Task<Result<IEnumerable<CardDto>>> GetActiveCardsByAccountIdAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<IEnumerable<CardDto>>.Failure(\"Geçersiz hesap ID'si\", 400);

                var cards = await _cardRepository.GetActiveCardsByAccountIdAsync(accountId);
                var cardDtos = _mapper.Map<IEnumerable<CardDto>>(cards);
                
                return Result<IEnumerable<CardDto>>.Success(cardDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Active kartları getirme hatası: {ex.Message}\");
                return Result<IEnumerable<CardDto>>.Failure(\"Bir hata oluştu\", 500);
            }
        }

        // ===== WRITE Operations =====

        public async Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request)
        {
            try
            {
                // Input validation
                if (request == null)
                    return Result<CardDto>.Failure(\"Request verisi boş olamaz\", 400);

                // Validate expiration date
                if (request.ExpirationDate <= DateTime.Now)
                    return Result<CardDto>.Failure(\"Son kullanma tarihi geçmiş olamaz\", 400);

                // Validate card number format
                if (!ValidateCardNumber(request.CardNumber))
                    return Result<CardDto>.Failure(\"Geçersiz kart numarası\", 400);

                // Check if account exists
                var account = await _accountRepository.GetByIdAsync(request.AccountId);
                if (account == null)
                    return Result<CardDto>.Failure(\"Hesap bulunamadı\", 404);

                // Check card number uniqueness
                if (await _cardRepository.CardNumberExistsAsync(request.CardNumber))
                    return Result<CardDto>.Failure(\"Bu kart numarası zaten kayıtlı\", 400);

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
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                var createdCard = await _cardRepository.CreateAsync(card);

                var cardDto = _mapper.Map<CardDto>(createdCard);
                _logger.LogInformation($\"Kart oluşturuldu: {card.Id}\");
                
                return Result<CardDto>.Success(
                    cardDto, 
                    \"Kart başarıyla oluşturuldu\", 
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Kart oluşturma hatası: {ex.Message}\");
                return Result<CardDto>.Failure(\"Kart oluşturulamadı\", 500);
            }
        }

        public async Task<Result<CardDto>> UpdateCardAsync(Guid cardId, UpdateCardRequest request)
        {
            try
            {
                if (cardId == Guid.Empty)
                    return Result<CardDto>.Failure(\"Geçersiz kart ID'si\", 400);

                var card = await _cardRepository.GetByIdAsync(cardId);
                if (card == null)
                    return Result<CardDto>.Failure(\"Kart bulunamadı\", 404);

                // Update allowed fields
                if (!string.IsNullOrEmpty(request.CardName))
                    card.CardName = request.CardName;

                if (request.Status.HasValue)
                    card.Status = request.Status.Value;

                card.UpdatedAt = DateTime.UtcNow;

                var updatedCard = await _cardRepository.UpdateAsync(card);
                var cardDto = _mapper.Map<CardDto>(updatedCard);
                
                _logger.LogInformation($\"Kart güncellendi: {cardId}\");
                return Result<CardDto>.Success(cardDto, \"Kart başarıyla güncellendi\", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Kart güncelleme hatası: {ex.Message}\");
                return Result<CardDto>.Failure(\"Kart güncellenemedi\", 500);
            }
        }

        // ===== DELETE Operations =====

        public async Task<Result<bool>> DeleteCardAsync(Guid cardId)
        {
            try
            {
                if (cardId == Guid.Empty)
                    return Result<bool>.Failure(\"Geçersiz kart ID'si\", 400);

                var exists = await _cardRepository.ExistsAsync(cardId);
                if (!exists)
                    return Result<bool>.Failure(\"Kart bulunamadı\", 404);

                var result = await _cardRepository.DeleteAsync(cardId);
                if (!result)
                    return Result<bool>.Failure(\"Kart silinemedi\", 500);

                _logger.LogInformation($\"Kart silindi: {cardId}\");
                return Result<bool>.Success(true, \"Kart başarıyla silindi\", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError($\"Kart silme hatası: {ex.Message}\");
                return Result<bool>.Failure(\"Kart silinemedi\", 500);
            }
        }

        // ===== Helper Methods =====

        /// <summary>
        /// Kart numarasını maskelenmiş formata çevir (1234 **** **** 6789)
        /// </summary>
        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 8)
                return cardNumber;

            var first4 = cardNumber.Substring(0, 4);
            var last4 = cardNumber.Substring(cardNumber.Length - 4);
            var masked = $\"{first4} **** **** {last4}\";

            return masked;
        }

        /// <summary>
        /// Kart numarası validasyonu (Luhn algoritması)
        /// </summary>
        private bool ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return false;

            // Remove spaces
            cardNumber = cardNumber.Replace(\" \", \"\");

            // Check if all digits
            if (!cardNumber.All(char.IsDigit))
                return false;

            // Check length
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Luhn algorithm
            int sum = 0;
            bool isSecond = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cardNumber[i].ToString());

                if (isSecond)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                isSecond = !isSecond;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// CVC'yi şifrele (Bu örnekte basit base64, production'da güvenli encryption kullan)
        /// </summary>
        private string EncryptCVC(string cvc)
        {
            try
            {
                // TODO: Implement proper AES encryption
                // This is just a placeholder
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cvc));
            }
            catch
            {
                return cvc; // Fallback (should not happen in production)
            }
        }

        /// <summary>
        /// CVC'yi deşifrele
        /// </summary>
        private string DecryptCVC(string encryptedCvc)
        {
            try
            {
                // TODO: Implement proper AES decryption
                // This is just a placeholder
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedCvc));
            }
            catch
            {
                return encryptedCvc; // Fallback
            }
        }
    }
}
```

---

## 5. AutoMapper Profile

```csharp
// Dosya: payment-system.Application/Common/Mappings/CardMappingProfile.cs

using AutoMapper;
using payment_system.Application.DTOs.Card;
using payment_system.Domain.Entities;

namespace payment_system.Application.Common.Mappings
{
    /// <summary>
    /// Card entity ile DTO'lar arasında mapping
    /// </summary>
    public class CardMappingProfile : Profile
    {
        public CardMappingProfile()
        {
            // Card Entity → CardDto
            CreateMap<Card, CardDto>()
                .ForMember(
                    dest => dest.CardNumber,
                    opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status));

            // CardDto → Card Entity (geriye dönüş)
            CreateMap<CardDto, Card>().ReverseMap();

            // CreateCardRequest → Card Entity
            CreateMap<CreateCardRequest, Card>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 8)
                return cardNumber;

            var first4 = cardNumber.Substring(0, 4);
            var last4 = cardNumber.Substring(cardNumber.Length - 4);
            return $\"{first4} **** **** {last4}\";
        }
    }
}
```

---

## 6. Controller

### CardController.cs
```csharp
// Dosya: payment-system.Api/Controllers/Card/CardController.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using payment_system.Application.DTOs.Card;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    /// <summary>
    /// Kart yönetimi API endpoints'i
    /// </summary>
    [ApiController]
    [Route(\"api/[controller]\")]
    [Authorize]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ILogger<CardController> _logger;

        public CardController(
            ICardService cardService,
            ILogger<CardController> logger)
        {
            _cardService = cardService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm kartları listele
        /// </summary>
        /// <returns>Kart listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAllCards()
        {
            _logger.LogInformation(\"Tüm kartlar listeleniyoruz\");
            var result = await _cardService.GetAllCardsAsync();

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Belirli bir kartı getir
        /// </summary>
        /// <param name=\"id\">Kart ID'si</param>
        /// <returns>Kart bilgileri</returns>
        [HttpGet(\"{id}\")]
        [ProducesResponseType(typeof(CardDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> GetCard(Guid id)
        {
            _logger.LogInformation($\"Kart getiriliyor: {id}\");
            var result = await _cardService.GetCardByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Account'a ait tüm kartları getir
        /// </summary>
        /// <param name=\"accountId\">Hesap ID'si</param>
        /// <returns>Kart listesi</returns>
        [HttpGet(\"account/{accountId}\")]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetCardsByAccount(Guid accountId)
        {
            _logger.LogInformation($\"Account {accountId} kartları getiriliyor\");
            var result = await _cardService.GetCardsByAccountIdAsync(accountId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Yeni kart oluştur
        /// </summary>
        /// <param name=\"request\">Kart oluşturma bilgileri</param>
        /// <returns>Oluşturulan kart</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CardDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardRequest request)
        {
            _logger.LogInformation(\"Yeni kart oluşturuluyor\");
            var result = await _cardService.CreateCardAsync(request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return CreatedAtAction(nameof(GetCard), new { id = result.Data?.Id }, result.Data);
        }

        /// <summary>
        /// Kartı güncelle
        /// </summary>
        /// <param name=\"id\">Kart ID'si</param>
        /// <param name=\"request\">Güncelleme bilgileri</param>
        /// <returns>Güncellenen kart</returns>
        [HttpPut(\"{id}\")]
        [ProducesResponseType(typeof(CardDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> UpdateCard(
            Guid id,
            [FromBody] UpdateCardRequest request)
        {
            _logger.LogInformation($\"Kart güncelleniyor: {id}\");
            var result = await _cardService.UpdateCardAsync(id, request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Kartı sil
        /// </summary>
        /// <param name=\"id\">Kart ID'si</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete(\"{id}\")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteCard(Guid id)
        {
            _logger.LogInformation($\"Kart siliniyor: {id}\");
            var result = await _cardService.DeleteCardAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return NoContent();
        }
    }
}
```

---

## 7. Dependency Injection Setup

```csharp
// Dosya: payment-system.Api/Extensions/ServiceCollectionExtension.cs
// İçinde güncellenecek kısım:

using Microsoft.Extensions.DependencyInjection;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Services.Implementations;
using payment_system.Application.Repositories;
using payment_system.Infrastructure.Repositories;

namespace payment_system.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Existing services...
            
            // ADD NEW - Card Services
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ICardRepository, CardRepository>();

            // AutoMapper
            services.AddAutoMapper(typeof(CardMappingProfile));

            return services;
        }
    }
}
```

---

**Created:** 11 Nisan 2026
**Status:** Ready for Development ✅

---

> 💡 **Sonraki Adımlar:**
> 1. Repository implementation'ı yazın
> 2. Bu kod örneklerini proje yapısına yerleştirin
> 3. Database migration'ı çalıştırın
> 4. Endpoints'i test edin
> 5. Unit tests yazın
