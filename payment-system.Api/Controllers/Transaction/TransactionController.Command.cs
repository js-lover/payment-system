using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    public partial class TransactionController : ControllerBase
    {
        /// <summary>
        /// Create a new transaction.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> Create(CreateTransactionRequest request)
        {
            var result = await _transactionService.CreateTransactionAsync(request);
            if (result.IsSuccess)
            {
                if (result.Data != null)
                    return CreatedAtAction(nameof(GetAll), new { id = result.Data.Id }, result.Data);

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Transaction created but result data is missing." });
            }
            return BadRequest(new { message = result.Message });
        }


    }
}