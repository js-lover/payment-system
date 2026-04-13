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
    /// Kartları görüntüleme, oluşturma ve silme işlemlerini handle eder
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Customer")]
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
        /// <response code="200">Başarılı</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAllCards()
        {
            _logger.LogInformation("Tüm kartlar listeleniyoruz");
            var result = await _cardService.GetAllCardsAsync();

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Belirli bir kartı getir
        /// </summary>
        /// <param name="id">Kart ID'si</param>
        /// <returns>Kart bilgileri (maskelenmiş CardNumber ile)</returns>
        /// <response code="200">Başarılı</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="404">Kart bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(CardDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> GetCard(Guid id)
        {
            _logger.LogInformation($"Kart getiriliyor: {id}");
            var result = await _cardService.GetCardByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Account'a ait tüm kartları getir
        /// </summary>
        /// <param name="accountId">Hesap ID'si</param>
        /// <returns>Kart listesi</returns>
        /// <response code="200">Başarılı</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="404">Hesap bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("account/{accountId}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetCardsByAccount(Guid accountId)
        {
            _logger.LogInformation($"Account {accountId} kartları getiriliyor");
            var result = await _cardService.GetCardsByAccountIdAsync(accountId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Yeni kart oluştur
        /// </summary>
        /// <param name="request">Kart oluşturma bilgileri</param>
        /// <returns>Oluşturulan kart (maskelenmiş CardNumber ile)</returns>
        /// <response code="201">Kart başarıyla oluşturuldu</response>
        /// <response code="400">Geçersiz istek</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="404">Hesap bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(CardDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardRequest request)
        {
            _logger.LogInformation("Yeni kart oluşturuluyor");
            var result = await _cardService.CreateCardAsync(request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return CreatedAtAction(nameof(GetCard), new { id = result.Data?.Id }, result.Data);
        }

        /// <summary>
        /// Kartı güncelle
        /// </summary>
        /// <param name="id">Kart ID'si</param>
        /// <param name="request">Güncelleme bilgileri</param>
        /// <returns>Güncellenen kart</returns>
        /// <response code="200">Başarılı</response>
        /// <response code="400">Geçersiz istek</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="404">Kart bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(CardDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> UpdateCard(
            Guid id,
            [FromBody] UpdateCardRequest request)
        {
            _logger.LogInformation($"Kart güncelleniyor: {id}");
            var result = await _cardService.UpdateCardAsync(id, request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Kartı sil (Soft Delete)
        /// </summary>
        /// <param name="id">Kart ID'si</param>
        /// <returns>Silme sonucu</returns>
        /// <response code="204">Kart başarıyla silindi</response>
        /// <response code="401">Yetkilendirme başarısız</response>
        /// <response code="404">Kart bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteCard(Guid id)
        {
            _logger.LogInformation($"Kart siliniyor: {id}");
            var result = await _cardService.DeleteCardAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return NoContent();
        }
    }
}
