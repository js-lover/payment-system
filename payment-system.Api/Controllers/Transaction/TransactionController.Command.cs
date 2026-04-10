using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    public partial class TransactionController : ControllerBase
    {
        ///<summary>
        /// Create a new transaction
        /// </summary>
        [HttpPost]
        [Authorize]  // ✅ GÜVENLIK: Sadece kimliği doğrulanmış kullanıcılar transaction yapabilir
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> Create(CreateTransactionRequest request)
        {
            var result = await _transactionService.CreateTransactionAsync(request);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetAll), new { id = result.Data.Id }, result.Data);
            }
            return BadRequest(new { message = result.Message });
        }

        
    }
}