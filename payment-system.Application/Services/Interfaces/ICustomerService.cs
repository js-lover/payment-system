using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Application.DTOs;
using payment_system.Application.DTOs.Customer;
using System.Collections.Generic;
using payment_system.Application.Common;

namespace payment_system.Application.Services.Interfaces
{
    /// <summary>
    /// Customer service contract
    /// defines all customer operations 
    /// independent from implementations, we use it for DI
    /// </summary 
    public interface ICustomerService
    {
        // ======= WRITE Operations =====

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="request">Customer data</param>
        /// <returns>Created customer ID</returns>
        Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request);

        // ======= READ Operations =====

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<CustomerDto>> GetCustomerByIdAsync(Guid id);

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<CustomerDto>>> GetAllCustomersAsync();


        //========= DELETE Operations ========

        Task<Result<CustomerDto>> DeleteCustomerAsync(Guid id);

    }
}