using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Customer;

namespace payment_system.Api.Controllers
{
    public partial class CustomerController
    {
        /// <summary>
        /// Retrieves a specific customer by their identifier.
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details if found</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]  // Security: Only Admin and Customer can access this endpoint
        public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            if(result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { message = result.Message });
        }

        /// <summary>
        /// Retrieves all customers from the system.
        /// </summary>
        /// <returns>List of all customers</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]  // Security: Admin and Customer can access this endpoint
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var result = await _customerService.GetAllCustomersAsync();
            if(result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { message = result.Message });
    }
    }
}