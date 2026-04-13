using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Infrastructure.Persistence.Contexts;

namespace payment_system.Infrastructure.Repositories
{
    /// <summary>
    /// Account repository implementation.
    /// Handles only account operations with no business logic.
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Constructor - DbContext is injected.
        /// </summary>
        /// <param name="db">Application DbContext</param>
        public AccountRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // ===== READ OPERATIONS =====

        /// <summary>
        /// Get account by ID.
        /// </summary>
        public async Task<Account?> GetByIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
                return null;

            return await _db.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }

        /// <summary>
        /// Get all accounts for a customer.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Account list</returns>
        public async Task<IEnumerable<Account>> GetAllByCustomerIdAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
                return Enumerable.Empty<Account>();

            return await _db.Accounts
                .Include(a => a.Customer)
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }



        /// <summary>
        /// Get account by customer ID.
        /// </summary>
        public async Task<Account?> GetByCustomerIdAsync(Guid customerId)
        {
            if (customerId == Guid.Empty)
                return null;

            return await _db.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Customer.Id == customerId);
        }

        /// <summary>
        /// Get all accounts.
        /// </summary>
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _db.Accounts
                .Include(a => a.Customer)
                .OrderBy(a => a.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Get accounts within a balance range.
        /// </summary>
        public async Task<IEnumerable<Account>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance)
        {
            if (minBalance < 0 || maxBalance < 0 || minBalance > maxBalance)
                return Enumerable.Empty<Account>();

            return await _db.Accounts
                .Include(a => a.Customer)
                .Where(a => a.Balance >= minBalance && a.Balance <= maxBalance)
                .OrderByDescending(a => a.Balance)
                .ToListAsync();
        }

        // ===== WRITE OPERATIONS =====

        /// <summary>
        /// Add a new account.
        /// </summary>
        public async Task AddAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            _db.Accounts.Add(account);
        }

        /// <summary>
        /// Update an existing account.
        /// </summary>
        public async Task UpdateAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var existingAccount = await _db.Accounts.FindAsync(account.Id);
            if (existingAccount == null)
                throw new InvalidOperationException($"Account with ID {account.Id} not found");

            _db.Accounts.Update(account);
        }

        /// <summary>
        /// Delete an account.
        /// </summary>
        public async Task DeleteAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
                return;

            var account = await GetByIdAsync(accountId);
            if (account != null)
            {
                _db.Accounts.Remove(account);
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