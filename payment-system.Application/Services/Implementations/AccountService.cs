using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Account;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Entities;

namespace payment_system.Application.Services.Implementations
{
    /// <summary>
    /// Account hizmetinin concrete implementasyonu
    /// İş mantığını içerir - validasyonlar, filtering vb
    /// Repository'ler sadece veri erişimi için kullanılır
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        /// <summary>
        /// Constructor - Dependency Injection
        /// </summary>
        public AccountService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        /// <summary>
        /// Account'un detaylı bilgisini ve transaction'larını getir
        /// İş mantığı: validasyon, data enrichment
        /// </summary>
        public async Task<Result<AccountDetailsDto>> GetAccountDetailsWithTransactionsAsync(Guid accountId)
        {
            // ===== İŞ MANTTIĞI: VALIDASYON =====
            
            if (accountId == Guid.Empty)
                return Result<AccountDetailsDto>.Failure(
                    "Invalid account ID",
                    400);

            // ===== İŞ MANTTIĞI: DATA FETCHING (Repository kullan) =====
            
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
                return Result<AccountDetailsDto>.Failure(
                    "Account not found",
                    404);

            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);

            // ===== İŞ MANTTIĞI: DTO MAPPING =====
            
            var accountDetailsDto = MapToAccountDetailsDto(account, transactions.ToList());

            return Result<AccountDetailsDto>.Success(
                accountDetailsDto,
                "Account details retrieved successfully",
                200);
        }

        /// <summary>
        /// Tüm account'ları detayları ile getir
        /// İş mantığı: veri zenginleştirme
        /// </summary>
        public async Task<Result<IEnumerable<AccountDetailsDto>>> GetAllAccountsAsync()
        {
            // ===== İŞ MANTTIĞI: DATA FETCHING =====
            
            var accounts = await _accountRepository.GetAllAsync();

            if (!accounts.Any())
                return Result<IEnumerable<AccountDetailsDto>>.Failure(
                    "No accounts found",
                    404);

            // ===== İŞ MANTTIĞI: VERI ZENGİNLEŞTİRME =====
            
            var accountDetailsList = new List<AccountDetailsDto>();

            foreach (var account in accounts)
            {
                var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                var accountDetailsDto = MapToAccountDetailsDto(account, transactions.ToList());
                accountDetailsList.Add(accountDetailsDto);
            }

            return Result<IEnumerable<AccountDetailsDto>>.Success(
                accountDetailsList,
                "All accounts retrieved successfully",
                200);
        }

        /// <summary>
        /// Belirli bakiye aralığındaki account'ları getir
        /// İş mantığı: validasyon, filtering
        /// </summary>
        public async Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByBalanceRangeAsync(
            decimal minBalance,
            decimal maxBalance)
        {
            // ===== İŞ MANTTIĞI: VALIDASYON =====
            
            if (minBalance < 0 || maxBalance < 0)
                return Result<IEnumerable<AccountDetailsDto>>.Failure(
                    "Balance values cannot be negative",
                    400);

            if (minBalance > maxBalance)
                return Result<IEnumerable<AccountDetailsDto>>.Failure(
                    "Minimum balance cannot be greater than maximum balance",
                    400);

            // ===== İŞ MANTTIĞI: DATA FETCHING (Repository filtering'i kullan) =====
            
            var accounts = await _accountRepository.GetAccountsByBalanceRangeAsync(minBalance, maxBalance);

            if (!accounts.Any())
                return Result<IEnumerable<AccountDetailsDto>>.Failure(
                    $"No accounts found with balance between {minBalance} and {maxBalance}",
                    404);

            // ===== İŞ MANTTIĞI: VERI ZENGİNLEŞTİRME =====
            
            var accountDetailsList = new List<AccountDetailsDto>();

            foreach (var account in accounts)
            {
                var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                var accountDetailsDto = MapToAccountDetailsDto(account, transactions.ToList());
                accountDetailsList.Add(accountDetailsDto);
            }

            return Result<IEnumerable<AccountDetailsDto>>.Success(
                accountDetailsList,
                "Accounts in balance range retrieved successfully",
                200);
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Helper: Account entity'sini AccountDetailsDto'ya dönüştür
        /// </summary>
        private AccountDetailsDto MapToAccountDetailsDto(Account account, List<Transaction> transactions)
        {
            return new AccountDetailsDto
            {
                Id = account.Id,
                Name = account.Customer?.Name ?? "Unknown Customer",
                Balance = account.Balance,
                Currency = account.Currency.ToString(),
                Transactions = transactions
                    .Select(MapToTransactionDto)
                    .ToList()
            };
        }

        /// <summary>
        /// Helper: Transaction entity'sini TransactionDto'ya dönüştür
        /// </summary>
        private TransactionDto MapToTransactionDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Currency = transaction.Currency.ToString(),
                TransactionStatus = transaction.Status.ToString(),
                Date = transaction.TransactionDate,
                TransactionType = transaction.TransactionType.ToString(),
                Description = transaction.Description,
                Children = transaction.ChildTransactions?
                    .Select(MapToTransactionDto)
                    .ToList() ?? new List<TransactionDto>()
            };
        }
    }
}