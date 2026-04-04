using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Account;

namespace payment_system.Application.Services.Interfaces
{
    /// <summary>
    /// Account service contract
    /// Tüm account operasyonlarını tanımlar
    /// Implementation'dan bağımsız, DI için kullanılır
    /// </summary>
    public interface IAccountService
    {
        // ===== READ OPERATIONS =====

        /// <summary>
        /// Tüm account'ları getir
        /// </summary>
        /// <returns>Account listesi veya hata</returns>
        Task<Result<IEnumerable<AccountDto>>> GetAllAccountsAsync();

        /// <summary>
        /// Account'un detaylı bilgisini ve transaction'larını getir
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Account detayları ve transaction'ları veya hata</returns>
        Task<Result<AccountDetailsDto>> GetAccountDetailsAsync(Guid accountId);

        /// <summary>
        /// Account bakiyesini getir
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Bakiye tutarı veya hata</returns>
        Task<Result<decimal>> GetAccountBalanceAsync(Guid accountId);

        /// <summary>
        /// Customer ID'sine göre account'u getir (Tekil)
        /// </summary>
        /// <param name="customerId">Customer ID'si</param>
        /// <returns>Account detayları veya hata</returns>
        Task<Result<AccountDetailsDto>> GetAccountByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Customer ID'sine göre account'ları getir (Çoklu)
        /// </summary>
        /// <param name="customerId">Customer ID'si</param>
        /// <returns>Account detayları veya hata</returns>
        Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Belirli bakiye aralığındaki account'ları getir
        /// </summary>
        /// <param name="minBalance">Minimum bakiye</param>
        /// <param name="maxBalance">Maximum bakiye</param>
        /// <returns>Filtrelenmiş account listesi veya hata</returns>
        Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance);

        // ===== WRITE OPERATIONS =====

        /// <summary>
        /// Yeni account oluştur
        /// </summary>
        /// <param name="request">Account oluşturma request'i</param>
        /// <returns>Oluşturulan account veya hata</returns>
        Task<Result<AccountDto>> CreateAccountAsync(CreateAccountRequest request);

        /// <summary>
        /// Account'u güncelle
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <param name="request">Güncelleme request'i</param>
        /// <returns>Güncellenen account veya hata</returns>
        Task<Result<AccountDto>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request);

        /// <summary>
        /// Account'u sil
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Başarı veya hata</returns>
        Task<Result<bool>> DeleteAccountAsync(Guid accountId);
    }
}