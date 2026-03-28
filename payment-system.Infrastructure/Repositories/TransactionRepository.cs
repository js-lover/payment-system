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
    /// Transaction repository implementasyonu
    /// Veritabanı operasyonlarını gerçekleştirir
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _db;

        public TransactionRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return await _db.Accounts.FindAsync(accountId);
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.ReferenceTransactionId == null)
                .ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _db.Transactions.Add(transaction);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}