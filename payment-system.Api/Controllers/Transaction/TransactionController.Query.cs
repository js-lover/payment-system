using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    public partial class TransactionController
    {
        /// <summary>
        /// Retrieves all transactions from the system.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]  // Security: Admin and Customer can access this endpoint
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
        /// Retrieves all transactions associated with a specific account.
        /// </summary>
        [HttpGet("account/{accountId}")]  // Route added
        [Authorize(Roles = "Admin,Customer")]  // Security: Admin and Customer can access this endpoint
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
        /// Retrieves transactions that occurred within the specified date range.
        /// </summary>
        [HttpGet("date-range")]  // Route added
        [Authorize(Roles = "Admin,Customer")]  // Security: Admin and Customer can access this endpoint
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
        /// Retrieves transactions filtered by transaction type.
        /// </summary>
        [HttpGet("type/{transactionType}")]  // Route added
        [Authorize(Roles = "Admin,Customer")]  // Security: Admin and Customer can access this endpoint
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
