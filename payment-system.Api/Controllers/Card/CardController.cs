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
    /// Card management API endpoints.
    /// Handles viewing, creating, and deleting cards.
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
        /// Get all cards.
        /// </summary>
        /// <returns>List of cards</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAllCards()
        {
            _logger.LogInformation("Retrieving all cards");
            var result = await _cardService.GetAllCardsAsync();

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get a specific card by ID.
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <returns>Card details with masked CardNumber</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Card not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(CardDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> GetCard(Guid id)
        {
            _logger.LogInformation($"Retrieving card: {id}");
            var result = await _cardService.GetCardByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Get all cards for a specific account.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>List of cards for the account</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("account/{accountId}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(IEnumerable<CardDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetCardsByAccount(Guid accountId)
        {
            _logger.LogInformation($"Retrieving cards for account: {accountId}");
            var result = await _cardService.GetCardsByAccountIdAsync(accountId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new card.
        /// </summary>
        /// <param name="request">Card creation request details</param>
        /// <returns>Created card with masked CardNumber</returns>
        /// <response code="201">Card created successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(typeof(CardDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardRequest request)
        {
            _logger.LogInformation("Creating new card");
            var result = await _cardService.CreateCardAsync(request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return CreatedAtAction(nameof(GetCard), new { id = result.Data?.Id }, result.Data);
        }

        /// <summary>
        /// Update an existing card.
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <param name="request">Update request details</param>
        /// <returns>Updated card</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Card not found</response>
        /// <response code="500">Server error</response>
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
            _logger.LogInformation($"Updating card: {id}");
            var result = await _cardService.UpdateCardAsync(id, request);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return Ok(result.Data);
        }

        /// <summary>
        /// Delete a card (soft delete).
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <returns>Deletion result</returns>
        /// <response code="204">Card deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Card not found</response>
        /// <response code="500">Server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteCard(Guid id)
        {
            _logger.LogInformation($"Deleting card: {id}");
            var result = await _cardService.DeleteCardAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 400, result.Message);

            return NoContent();
        }
    }
}
