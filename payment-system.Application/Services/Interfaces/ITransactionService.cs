using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Transaction;

namespace payment_system.Application.Services.Interfaces
{
    /// <summary>
    /// Transaction service contract
    /// Tüm transaction operasyonlarını tanımlar
    /// Implementation'dan bağımsız, DI için kullanılır
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Yeni transaction oluştur
        /// Tüm validasyonları gerçekleştirir, iş mantığını uygular
        /// </summary>
        /// <param name="request">Transaction oluşturma request'i</param>
        /// <returns>
        /// Success: Oluşturulan transaction (201)
        /// BadRequest: Validasyon hatası (400)
        /// NotFound: Account bulunamadı (404)
        /// </returns>
        Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request);

        /// <summary>
        /// Tüm transaction'ları getir (parent-child ilişkisi ile)
        /// </summary>
        /// <returns>Transaction listesi</returns>
        Task<Result<IEnumerable<TransactionDto>>> GetAllTransactionsAsync();

        /// <summary>
        /// Belirli account'a ait transaction'ları getir
        /// </summary>
        /// <param name="accountId">Account ID'si</param>
        /// <returns>Account'ın transaction listesi</returns>
        Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByAccountIdAsync(Guid accountId);

        /// <summary>
        /// Belirli tarih aralığında transaction'ları getir
        /// </summary>
        /// <param name="startDate">Başlangıç tarihi</param>
        /// <param name="endDate">Bitiş tarihi</param>
        /// <returns>Filtrelenmiş transaction listesi</returns>
        Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Belirli transaction tipine göre transaction'ları getir
        /// </summary>
        /// <param name="transactionType">Transaction tipi (Sale, Refund, Purchase)</param>
        /// <returns>Filtrelenmiş transaction listesi</returns>
        Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByTypeAsync(string transactionType);
    }
}