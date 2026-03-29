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
    /// Transaction repository implementasyonu
    /// Sadece Transaction operasyonlarını gerçekleştirir
    /// İş mantığı yoktur - sadece veri erişimi
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Constructor - DbContext inject edilir
        /// </summary>
        /// <param name="db">Application DbContext</param>
        public TransactionRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // ===== READ OPERATIONS =====

        /// <summary>
        /// ID'ye göre transaction'ı getir
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
        /// Tüm transaction'ları getir (parent transaction'ları)
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _db.Transactions
                .Include(t => t.ChildTransactions)
                .Where(t => t.ReferenceTransactionId == null)  // ← Sadece parent transaction'lar
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Belirli account'a ait transaction'ları getir
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
        /// Tarih aralığında transaction'ları getir
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
        /// Transaction tipine göre transaction'ları getir
        /// </summary>
        public async Task<IEnumerable<Transaction>> GetByTransactionTypeAsync(string transactionType)
        {
            // Validasyon: string'i enum'a dönüştür
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
        /// Yeni transaction ekle
        /// </summary>
        public async Task AddAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _db.Transactions.Add(transaction);
        }

        /// <summary>
        /// Mevcut transaction'ı güncelle
        /// </summary>
        public async Task UpdateAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            // Mevcut transaction var mı?
            var existingTransaction = await _db.Transactions.FindAsync(transaction.Id);
            if (existingTransaction == null)
                throw new InvalidOperationException($"Transaction with ID {transaction.Id} not found");

            _db.Transactions.Update(transaction);
        }

        /// <summary>
        /// Transaction'ı sil
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
        /// Veritabanındaki tüm değişiklikleri kaydet
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}