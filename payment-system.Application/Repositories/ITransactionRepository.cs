using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Transaction repository interface.
    /// Defines only transaction operations independent of infrastructure.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Get transaction by ID (with child transactions).
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transaction entity or null</returns>
        Task<Transaction?> GetByIdAsync(Guid transactionId);

        /// <summary>
        /// Get all transactions (parent transactions with children).
        /// </summary>
        /// <returns>Transaction list</returns>
        Task<IEnumerable<Transaction>> GetAllAsync();

        /// <summary>
        /// Get transactions for a specific account.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Transactions for the account</returns>
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Get transactions within a date range.
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Filtered transaction list</returns>
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get transactions by type (Sale, Refund, etc).
        /// </summary>
        /// <param name="transactionType">Transaction type as string</param>
        /// <returns>Filtered transaction list</returns>
        Task<IEnumerable<Transaction>> GetByTransactionTypeAsync(string transactionType);

        /// <summary>
        /// Add a new transaction.
        /// </summary>
        /// <param name="transaction">Transaction entity to add</param>
        /// <returns>Task</returns>
        Task AddAsync(Transaction transaction);

        /// <summary>
        /// Update an existing transaction.
        /// </summary>
        /// <param name="transaction">Transaction entity to update</param>
        /// <returns>Task</returns>
        Task UpdateAsync(Transaction transaction);

        /// <summary>
        /// Delete a transaction.
        /// </summary>
        /// <param name="transactionId">Transaction ID to delete</param>
        /// <returns>Task</returns>
        Task DeleteAsync(Guid transactionId);

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        /// <returns>Task</returns>
        Task SaveChangesAsync();
    }
}