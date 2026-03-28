using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Transaction;

namespace payment_system.Application.Services.Interfaces
{
    /// <summary>
    /// Transaction service contract
    /// Tüm transaction operasyonlarını tanımlar
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Yeni transaction oluştur
        /// </summary>
        /// <param name="request">Transaction request DTO</param>
        /// <returns>Oluşturulan transaction DTO veya hata</returns>
        Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request);
                //   ↑                       ↑
                //   Dönüş tipi              Metod imzası
        /// <summary>
        /// Tüm transaction'ları getir
        /// </summary>
        /// <returns>Transaction listesi veya hata</returns>
        Task<Result<IEnumerable<TransactionDto>>> GetAllTransactionsAsync();
    }
}