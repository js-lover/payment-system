using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    /// <summary>
    /// Transaction repository interface - veri erişim abstraktı
    /// Infrastructure'dan bağımsız
    /// </summary>
    public interface ITransactionRepository
    {
        Task<Account?> GetAccountByIdAsync(Guid accountId);
        Task<Transaction?> GetTransactionByIdAsync(Guid transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}