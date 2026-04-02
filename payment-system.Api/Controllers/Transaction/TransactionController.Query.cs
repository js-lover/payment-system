using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    public partial class TransactionController
    {
        /// <summary>
        /// Tüm transaction'ları getir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Belirli account'a ait transaction'ları getir
        /// </summary>
        [HttpGet("account/{accountId}")]  // ✅ Route eklendi
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByAccountId(Guid accountId)
        {
            var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Tarih aralığında transaction'ları getir
        /// </summary>
        [HttpGet("date-range")]  // ✅ Route eklendi
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Transaction tipine göre getir
        /// </summary>
        [HttpGet("type/{transactionType}")]  // ✅ Route eklendi
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByTransactionType(string transactionType)
        {
            var result = await _transactionService.GetTransactionsByTypeAsync(transactionType);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }
    }
}
