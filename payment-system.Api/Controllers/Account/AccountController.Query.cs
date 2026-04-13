using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Account;

namespace payment_system.Api.Controllers
{
    public partial class AccountController
    {
        /// <summary>
        /// Retrieves all accounts from the system.
        /// </summary>
        [Authorize(Roles = "Admin,Customer")]  // Security: Only Admin and Customer can access this endpoint
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
        /// Retrieves detailed account information including associated transactions.
        /// </summary>
        [HttpGet("{accountId}/details")]
        [Authorize(Roles = "Admin")]
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
        /// Retrieves the current balance of an account.
        /// </summary>
        [HttpGet("{accountId}/balance")]
        [Authorize(Roles = "Admin")]
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
        /// Retrieves an account by customer identifier.
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin")]
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
        /// Retrieves accounts whose balance falls within the specified range.
        /// </summary>
        [HttpGet("balance-range")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AccountDetailsDto>>> GetByBalanceRange(
            [FromQuery] decimal minBalance,
            [FromQuery] decimal maxBalance)
        {
            if (minBalance < 0 || maxBalance < 0 || minBalance > maxBalance)
                return BadRequest(new { message = "Invalid balance range." });

            var result = await _accountService.GetAccountsByBalanceRangeAsync(minBalance, maxBalance);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }


    }
}