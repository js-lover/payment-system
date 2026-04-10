using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using payment_system.Application.DTOs.Account;

namespace payment_system.Api.Controllers
{
    public partial class AccountController
    {
        /// <summary>
        /// Yeni account oluştur
        /// </summary>
        [HttpPost]
        [Authorize]  // ✅ GÜVENLIK: Sadece kimliği doğrulanmış kullanıcılar oluşturabilir
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request)
        {
            var result = await _accountService.CreateAccountAsync(request);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetDetails), new { accountId = result.Data.Id }, result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Account'u güncelle
        /// </summary>
        [HttpPut("{accountId}")]
        [Authorize]  // ✅ GÜVENLIK: Sadece kimliği doğrulanmış kullanıcılar güncelleyebilir
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountDto>> Update(Guid accountId, [FromBody] UpdateAccountRequest request)
        {
            var result = await _accountService.UpdateAccountAsync(accountId, request);
            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Account'u sil
        /// </summary>
        [HttpDelete("{accountId}")]
        [Authorize]  // ✅ GÜVENLIK: Sadece kimliği doğrulanmış kullanıcılar silebilir
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid accountId)
        {
            var result = await _accountService.DeleteAccountAsync(accountId);
            if (result.IsSuccess)
                return NoContent();

            return NotFound(new { message = result.Message });
        }
    }
}