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
        /// <summary>
        /// Account'un detaylı bilgisini ve transaction'larını getir
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Account detayları ve transaction'ları veya hata</returns>
        Task<Result<AccountDetailsDto>> GetAccountDetailsWithTransactionsAsync(Guid accountId);

        /// <summary>
        /// Tüm account'ları detayları ile getir
        /// Her account'un transaction'larını da yükler
        /// </summary>
        /// <returns>Account listesi veya hata</returns>
        Task<Result<IEnumerable<AccountDetailsDto>>> GetAllAccountsAsync();

        /// <summary>
        /// Belirli bakiye aralığındaki account'ları getir
        /// </summary>
        /// <param name="minBalance">Minimum bakiye</param>
        /// <param name="maxBalance">Maximum bakiye</param>
        /// <returns>Filtrelenmiş account listesi veya hata</returns>
        Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance);
    }
}