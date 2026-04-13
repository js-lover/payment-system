using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Application.DTOs.Account;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Account repository interface.
    /// Defines only account operations independent of infrastructure.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Get account by ID (with customer information).
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Account entity or null</returns>
        Task<Account?> GetByIdAsync(Guid accountId);

        /// <summary>
        /// Get account by customer ID.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Account entity or null</returns>
        Task<Account?> GetByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Get all accounts for a customer.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Account list</returns>
        Task<IEnumerable<Account>> GetAllByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Get all accounts.
        /// </summary>
        /// <returns>Account list</returns>
        Task<IEnumerable<Account>> GetAllAsync();

        /// <summary>
        /// Get accounts within a balance range.
        /// </summary>
        /// <param name="minBalance">Minimum balance</param>
        /// <param name="maxBalance">Maximum balance</param>
        /// <returns>Filtered account list</returns>
        Task<IEnumerable<Account>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance);

        /// <summary>
        /// Add a new account.
        /// </summary>
        /// <param name="account">Account entity to add</param>
        /// <returns>Task</returns>
        Task AddAsync(Account account);

        /// <summary>
        /// Update an existing account.
        /// </summary>
        /// <param name="account">Account entity to update</param>
        /// <returns>Task</returns>
        Task UpdateAsync(Account account);

        /// <summary>
        /// Delete an account.
        /// </summary>
        /// <param name="accountId">Account ID to delete</param>
        /// <returns>Task</returns>
        Task DeleteAsync(Guid accountId);

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        /// <returns>Task</returns>
        Task SaveChangesAsync();
    }
}