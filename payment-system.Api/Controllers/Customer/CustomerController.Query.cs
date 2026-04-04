using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Customer;

namespace payment_system.Api.Controllers
{
    public partial class CustomerController
    {
        ///<summary>
        /// get customer by id
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            if(result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var result = await _customerService.GetAllCustomersAsync();
            if(result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { message = result.Message });
    }
    }
}