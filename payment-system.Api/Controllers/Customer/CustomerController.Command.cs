using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.DTOs.Customer;
using payment_system.Domain.Entities;
using payment_system.Application.DTOs;

namespace payment_system.Api.Controllers
{
    public partial class CustomerController
    {

        /// <summary>
        /// Creates a new customer record in the system.
        /// </summary>
        /// <param name="request">Customer creation request with required information</param>
        /// <returns>Created customer with HTTP 201 status</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]  // Security: Only Admin and Customer can create customers
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
        [Authorize(Roles = "Admin,Customer")]  // Security: Only Admin and Customer can delete customers
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