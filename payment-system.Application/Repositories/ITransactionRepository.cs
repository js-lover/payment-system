using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Transaction repository interface
    /// Sadece Transaction operasyonlarını tanımlar
    /// Infrastructure'dan bağımsız
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// ID'ye göre transaction'ı getir (child transaction'ları ile birlikte)
        /// </summary>
        /// <param name="transactionId">Transaction ID'si</param>
        /// <returns>Transaction entity veya null</returns>
        Task<Transaction?> GetByIdAsync(Guid transactionId);

        /// <summary>
        /// Tüm transaction'ları getir (parent transaction'ları, child'ları ile)
        /// </summary>
        /// <returns>Transaction listesi</returns>
        Task<IEnumerable<Transaction>> GetAllAsync();

        /// <summary>
        /// Belirli account'a ait transaction'ları getir
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Account'a ait transaction listesi</returns>
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Tarih aralığında transaction'ları getir
        /// </summary>
        /// <param name="startDate">Başlangıç tarihi</param>
        /// <param name="endDate">Bitiş tarihi</param>
        /// <returns>Filtrelenmiş transaction listesi</returns>
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Transaction tipine göre transaction'ları getir (Sale, Refund, vb)
        /// </summary>
        /// <param name="transactionType">Transaction tipi string olarak</param>
        /// <returns>Filtrelenmiş transaction listesi</returns>
        Task<IEnumerable<Transaction>> GetByTransactionTypeAsync(string transactionType);

        /// <summary>
        /// Yeni transaction ekle
        /// </summary>
        /// <param name="transaction">Eklenecek transaction entity'si</param>
        /// <returns>Task</returns>
        Task AddAsync(Transaction transaction);

        /// <summary>
        /// Mevcut transaction'ı güncelle
        /// </summary>
        /// <param name="transaction">Güncellenecek transaction entity'si</param>
        /// <returns>Task</returns>
        Task UpdateAsync(Transaction transaction);

        /// <summary>
        /// Transaction'ı sil
        /// </summary>
        /// <param name="transactionId">Silinecek transaction'un ID'si</param>
        /// <returns>Task</returns>
        Task DeleteAsync(Guid transactionId);

        /// <summary>
        /// Veritabanındaki tüm değişiklikleri kaydet
        /// </summary>
        /// <returns>Task</returns>
        Task SaveChangesAsync();
    }
}