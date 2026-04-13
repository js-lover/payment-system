using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;
using payment_system.Infrastructure.Persistence.Contexts;

namespace payment_system.Infrastructure.Repositories
{
    /// <summary>
    /// Transaction repository implementation.
    /// Handles only transaction operations with no business logic.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Constructor - DbContext is injected.
        /// </summary>
        /// <param name="db">Application DbContext</param>
        public TransactionRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // ===== READ OPERATIONS =====

        /// <summary>
        /// Get transaction by ID.
        /// </summary>
        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
                return null;

            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        /// <summary>
        /// Get all transactions (parent transactions only).
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.ReferenceTransactionId == null)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get transactions for a specific account.
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
                return Enumerable.Empty<Transaction>();

            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.AccountId == accountId && t.ReferenceTransactionId == null)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get transactions within a date range.
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return Enumerable.Empty<Transaction>();

            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get transactions by transaction type.
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByTransactionTypeAsync(string transactionType)
        {
            // Convert string to enum
            if (!Enum.TryParse<TransactionType>(transactionType, ignoreCase: true, out var type))
                return Enumerable.Empty<Transaction>();

            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.TransactionType == type)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        // ===== WRITE OPERATIONS =====

        /// <summary>
        /// Add a new transaction.
        /// </summary>
        public async Task AddAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _db.Transactions.Add(transaction);
        }

        /// <summary>
        /// Update an existing transaction.
        /// </summary>
        public async Task UpdateAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            var existingTransaction = await _db.Transactions.FindAsync(transaction.Id);
            if (existingTransaction == null)
                throw new InvalidOperationException($"Transaction with ID {transaction.Id} not found");

            _db.Transactions.Update(transaction);
        }

        /// <summary>
        /// Delete a transaction.
        /// </summary>
        public async Task DeleteAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
                return;

            var transaction = await GetByIdAsync(transactionId);
            if (transaction != null)
            {
                _db.Transactions.Remove(transaction);
            }
        }

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}