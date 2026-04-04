using Microsoft.AspNetCore.Mvc;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.DTOs.Customer;
using payment_system.Domain.Entities;
using payment_system.Application.DTOs;

namespace payment_system.Api.Controllers
{
    public partial class CustomerController
    {

        /// <summary>
        /// Yeni bir müşteri oluşturur.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var result = await _customerService.CreateCustomerAsync(request);
            if (result.IsSuccess && result.Data != null)
                return CreatedAtAction("GetCustomerById", new { id = result.Data.Id }, result.Data);

            return BadRequest(new { message = result.Message });
        }




        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status200OK, "Customer deleted successfully.");

            return NotFound(new { message = result.Message });
        }
    }
}