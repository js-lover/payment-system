using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Account repository interface
    /// Sadece Account operasyonlarını tanımlar
    /// Infrastructure'dan bağımsız
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// ID'ye göre account'u getir (Customer bilgisi ile birlikte)
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Account entity veya null</returns>
        Task<Account?> GetByIdAsync(Guid accountId);

        /// <summary>
        /// Customer ID'sine göre account'u getir
        /// </summary>
        /// <param name="customerId">Customer ID'si</param>
        /// <returns>Account entity veya null</returns>
        Task<Account?> GetByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Tüm account'ları getir
        /// </summary>
        /// <returns>Account listesi</returns>
        Task<IEnumerable<Account>> GetAllAsync();

        /// <summary>
        /// Belirli bakiye aralığındaki account'ları getir
        /// </summary>
        /// <param name="minBalance">Minimum bakiye</param>
        /// <param name="maxBalance">Maximum bakiye</param>
        /// <returns>Filtrelenmiş account listesi</returns>
        Task<IEnumerable<Account>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance);

        /// <summary>
        /// Yeni account ekle
        /// </summary>
        /// <param name="account">Eklenecek account entity'si</param>
        /// <returns>Task</returns>
        Task AddAsync(Account account);

        /// <summary>
        /// Mevcut account'u güncelle
        /// </summary>
        /// <param name="account">Güncellenecek account entity'si</param>
        /// <returns>Task</returns>
        Task UpdateAsync(Account account);

        /// <summary>
        /// Account'u sil
        /// </summary>
        /// <param name="accountId">Silinecek account'un ID'si</param>
        /// <returns>Task</returns>
        Task DeleteAsync(Guid accountId);

        /// <summary>
        /// Veritabanındaki tüm değişiklikleri kaydet
        /// </summary>
        /// <returns>Task</returns>
        Task SaveChangesAsync();
    }
}