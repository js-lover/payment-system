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
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        // ===== HELPER METHODS =====

        private AccountDto MapToAccountDto(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                Name = account.Name,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Currency = account.Currency.ToString()
            };
        }

        private AccountDetailsDto MapToAccountDetailsDto(Account account, IEnumerable<Transaction> transactions)
        {
            return new AccountDetailsDto
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                Name = account.Name,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Currency = account.Currency.ToString(),
                Transactions = transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TransactionStatus = t.Status.ToString() ?? "Unknown",  // ✅ TransactionStatus → Status
                    TransactionType = t.TransactionType.ToString(),
                    Date = t.CreatedAt,
                    Description = t.Description,
                    Currency = t.Currency.ToString()
                }).ToList()
            };
        }

        // ===== READ OPERATIONS =====

        public async Task<Result<IEnumerable<AccountDto>>> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await _accountRepository.GetAllAsync();
                var accountDtos = accounts.Select(MapToAccountDto).ToList();
                return Result<IEnumerable<AccountDto>>.Success(accountDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AccountDto>>.Failure($"Hesaplar alınırken hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<AccountDetailsDto>> GetAccountDetailsAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<AccountDetailsDto>.Failure("Geçersiz account ID'si");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<AccountDetailsDto>.Failure("Account bulunamadı");

                var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
                var result = MapToAccountDetailsDto(account, transactions);

                return Result<AccountDetailsDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<AccountDetailsDto>.Failure($"Account detayları alınırken hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<AccountDetailsDto>> GetAccountByCustomerIdAsync(Guid customerId)
        {
            try
            {
                if (customerId == Guid.Empty)
                    return Result<AccountDetailsDto>.Failure("Geçersiz customer ID'si");

                var account = await _accountRepository.GetByCustomerIdAsync(customerId);
                if (account == null)
                    return Result<AccountDetailsDto>.Failure("Bu müşteriye ait account bulunamadı");

                var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                var result = MapToAccountDetailsDto(account, transactions);

                return Result<AccountDetailsDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<AccountDetailsDto>.Failure($"Müşteri account'u alınırken hata oluştu: {ex.Message}");
            }
        }


        /// <summary>
        /// Müşteri ID'sine göre tüm hesapları getirir.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByCustomerIdAsync(Guid customerId)
        {
            try
            {
                if (customerId == Guid.Empty)
                    return Result<IEnumerable<AccountDetailsDto>>.Failure("Geçersiz customer ID'si");

                var accounts = await _accountRepository.GetAllByCustomerIdAsync(customerId);

                var result = new List<AccountDetailsDto>();
                foreach (var account in accounts)
                {
                    var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                    result.Add(MapToAccountDetailsDto(account, transactions));
                }

                return Result<IEnumerable<AccountDetailsDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AccountDetailsDto>>.Failure($"Müşteri account'ları alınırken hata oluştu: {ex.Message}");
            }
        }




        public async Task<Result<decimal>> GetAccountBalanceAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<decimal>.Failure("Geçersiz account ID'si");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<decimal>.Failure("Account bulunamadı");

                return Result<decimal>.Success(account.Balance);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure($"Bakiye alınırken hata oluştu: {ex.Message}");
            }
        }


        public async Task<Result<IEnumerable<AccountDetailsDto>>> GetAccountsByBalanceRangeAsync(decimal minBalance, decimal maxBalance)
        {
            try
            {
                if (minBalance < 0 || maxBalance < 0 || minBalance > maxBalance)
                    return Result<IEnumerable<AccountDetailsDto>>.Failure("Geçersiz bakiye aralığı");

                var accounts = await _accountRepository.GetAllAsync();
                var filteredAccounts = accounts.Where(a => a.Balance >= minBalance && a.Balance <= maxBalance).ToList();

                var result = new List<AccountDetailsDto>();
                foreach (var account in filteredAccounts)
                {
                    var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                    result.Add(MapToAccountDetailsDto(account, transactions));
                }

                return Result<IEnumerable<AccountDetailsDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AccountDetailsDto>>.Failure($"Bakiye aralığına göre account'lar alınırken hata oluştu: {ex.Message}");
            }
        }

        // ===== WRITE OPERATIONS =====

        public async Task<Result<AccountDto>> CreateAccountAsync(CreateAccountRequest request)
        {
            try
            {
                if (request == null)
                    return Result<AccountDto>.Failure("Account request boş olamaz");

                if (request.CustomerId == Guid.Empty)
                    return Result<AccountDto>.Failure("Geçersiz customer ID'si");

                if (string.IsNullOrWhiteSpace(request.Name))
                    return Result<AccountDto>.Failure("Account adı boş olamaz");

                var accountNumber = GenerateAccountNumber();

                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Name = request.Name,
                    AccountNumber = accountNumber,
                    Balance = request.InitialBalance,
                    Currency = Enum.Parse<Domain.Enums.Currency>(request.Currency),
                    CreatedAt = DateTime.UtcNow
                };

                await _accountRepository.AddAsync(account);
                await _accountRepository.SaveChangesAsync();

                return Result<AccountDto>.Success(MapToAccountDto(account));
            }
            catch (Exception ex)
            {
                var fullMessage = ex.InnerException?.Message ?? ex.Message;
                return Result<AccountDto>.Failure($"Account oluşturulurken hata oluştu: {fullMessage}");

            }
        }

        private string GenerateAccountNumber()
        {
            // ✅ Format: TR + 18 sayısal karakter = 20 karakter
            // Constraint: length = 20 AND substr(..., 1, 2) = 'TR'

            // Unique ID oluştur (Ticks + Random)
            var random = new System.Random();
            var uniqueNumber = $"{DateTime.UtcNow.Ticks}{random.Next(1000, 9999)}";

            // İlk 18 karakteri al (garantili sayısal)
            var numberPart = uniqueNumber.Substring(0, 18);

            // Format: TR + 18 sayı = 20 karakter
            return $"TR{numberPart}";
        }

        public async Task<Result<AccountDto>> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<AccountDto>.Failure("Geçersiz account ID'si");

                if (request == null)
                    return Result<AccountDto>.Failure("Update request boş olamaz");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<AccountDto>.Failure("Account bulunamadı");

                if (!string.IsNullOrWhiteSpace(request.Name))
                    account.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Currency))
                    account.Currency = Enum.Parse<Domain.Enums.Currency>(request.Currency);

                await _accountRepository.SaveChangesAsync();

                return Result<AccountDto>.Success(MapToAccountDto(account));
            }
            catch (Exception ex)
            {
                return Result<AccountDto>.Failure($"Account güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAccountAsync(Guid accountId)
        {
            try
            {
                if (accountId == Guid.Empty)
                    return Result<bool>.Failure("Geçersiz account ID'si");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return Result<bool>.Failure("Account bulunamadı");

                await _accountRepository.DeleteAsync(accountId);  // ✅ account yerine accountId
                await _accountRepository.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Account silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}