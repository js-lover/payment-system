using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Account;

namespace payment_system.Api.Controllers
{
    public partial class AccountController
    {
        /// <summary>
        /// Tüm account'ları getir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AccountDetailsDto>>> GetAll()
        {
            var result = await _accountService.GetAllAccountsAsync();
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Account detaylarını ve transaction'larını getir
        /// </summary>
        [HttpGet("{accountId}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDetailsDto>> GetDetails(Guid accountId)
        {
            var result = await _accountService.GetAccountDetailsAsync(accountId);
            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Account bakiyesini getir
        /// </summary>
        [HttpGet("{accountId}/balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetBalance(Guid accountId)
        {
            var result = await _accountService.GetAccountBalanceAsync(accountId);
            if (result.IsSuccess)
                return Ok(new { balance = result.Data });

            return NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Customer ID'sine göre account'u getir
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDetailsDto>> GetByCustomerId(Guid customerId)
        {
            var result = await _accountService.GetAccountByCustomerIdAsync(customerId);
            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Bakiye aralığına göre account'ları getir
        /// </summary>
        [HttpGet("balance-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AccountDetailsDto>>> GetByBalanceRange(
            [FromQuery] decimal minBalance,
            [FromQuery] decimal maxBalance)
        {
            if (minBalance < 0 || maxBalance < 0 || minBalance > maxBalance)
                return BadRequest(new { message = "Geçersiz bakiye aralığı." });

            var result = await _accountService.GetAccountsByBalanceRangeAsync(minBalance, maxBalance);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }
    }
}