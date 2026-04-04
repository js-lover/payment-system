using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;

namespace payment_system.Application.Services.Implementations
{
    /// <summary>
    /// Transaction hizmetinin concrete implementasyonu
    /// İş mantığını içerir - validasyonlar, hesaplamalar vb
    /// Repository'ler sadece veri erişimi için kullanılır
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        /// <summary>
        /// Constructor - Dependency Injection aracılığıyla Repository'ler injekte edilir
        /// </summary>
        public TransactionService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        /// <summary>
        /// Yeni transaction oluştur - İş mantığı burada!
        /// </summary>
        public async Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request)
        {
            // ===== İŞ MANTTIĞI: TEMEL VALIDASYONLAR =====
            
            if (request.Amount <= 0)
                return Result<TransactionDto>.Failure(
                    "Amount must be greater than zero.",
                    400);

            // ===== İŞ MANTTIĞI: ACCOUNT KONTROL =====
            
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return Result<TransactionDto>.Failure(
                    "Account not found.",
                    404);

            // ===== İŞ MANTTIĞI: TRANSACTION TİPİNE GÖRE KONTROL =====

            switch (request.TransactionType)
            {
                case TransactionType.Sale:
                    return await ProcessSaleTransactionAsync(request, account);

                case TransactionType.Refund:
                    return await ProcessRefundTransactionAsync(request, account);

                default:
                    return Result<TransactionDto>.Failure(
                        $"Unknown transaction type: {request.TransactionType}",
                        400);
            }
        }

        /// <summary>
        /// SALE transaction işlemi - İş mantığı
        /// Bakiyeyi düş, transaction oluştur, kaydet
        /// </summary>
        private async Task<Result<TransactionDto>> ProcessSaleTransactionAsync(
            CreateTransactionRequest request,
            Account account)
        {
            // ===== İŞ MANTTIĞI: BAKIYE KONTROLÜ =====
            
            if (account.Balance < request.Amount)
                return Result<TransactionDto>.Failure(
                    $"Insufficient balance. Available: {account.Balance}, Required: {request.Amount}",
                    400);

            // ===== İŞ MANTTIĞI: BAKIYE GÜNCELLEME =====
            
            account.Balance -= request.Amount;

            // ===== İŞ MANTTIĞI: TRANSACTION OLUŞTURMA =====
            
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                TransactionType = request.TransactionType,
                Description = request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.Success,
                ReferenceTransactionId = null
            };

            // ===== İŞ MANTTIĞI: VERITABANINA KAYDET =====
            
            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.SaveChangesAsync();

            return Result<TransactionDto>.Success(
                MapToDto(transaction),
                "Sale transaction created successfully",
                201);
        }

        /// <summary>
        /// REFUND transaction işlemi - İş mantığı
        /// Refund validasyonu, bakiye geri yükleme, child transaction oluşturma
        /// </summary>
        private async Task<Result<TransactionDto>> ProcessRefundTransactionAsync(
            CreateTransactionRequest request,
            Account account)
        {
            // ===== İŞ MANTTIĞI: REFUND VALIDASYONLARI =====
            
            // Reference transaction gerekli
            if (!request.ReferenceTransactionId.HasValue)
                return Result<TransactionDto>.Failure(
                    "Refund transactions must reference an existing transaction.",
                    400);

            // Reference transaction var mı?
            var referenceTransaction = await _transactionRepository.GetByIdAsync(
                request.ReferenceTransactionId.Value);

            if (referenceTransaction == null)
                return Result<TransactionDto>.Failure(
                    "Invalid reference transaction.",
                    404);

            // Reference transaction aynı account'a mı ait?
            if (referenceTransaction.AccountId != request.AccountId)
                return Result<TransactionDto>.Failure(
                    "Cannot refund a transaction from another account.",
                    400);

            // Reference transaction SALE mı?
            if (referenceTransaction.TransactionType != TransactionType.Sale)
                return Result<TransactionDto>.Failure(
                    "Only sale transactions can be refunded.",
                    400);

            // ===== İŞ MANTTIĞI: TOPLAM REFUND KONTROLÜ =====
            
            var totalRefundAmount = referenceTransaction.ChildTransactions?
                .Sum(x => x.Amount) ?? 0;

            if (totalRefundAmount + request.Amount > referenceTransaction.Amount)
                return Result<TransactionDto>.Failure(
                    $"Refund amount exceeds available amount. " +
                    $"Available: {referenceTransaction.Amount - totalRefundAmount}, " +
                    $"Requested: {request.Amount}",
                    400);

            // ===== İŞ MANTTIĞI: BAKIYE GERI YÜKLEME =====
            
            account.Balance += request.Amount;

            // ===== İŞ MANTTIĞI: REFUND TRANSACTION OLUŞTURMA =====
            
            var refundTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                TransactionType = request.TransactionType,
                Description = request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.Success,
                ReferenceTransactionId = referenceTransaction.Id  // ← Parent'a referans
            };

            // ===== İŞ MANTTIĞI: VERITABANINA KAYDET =====
            
            await _transactionRepository.AddAsync(refundTransaction);
            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.SaveChangesAsync();

            return Result<TransactionDto>.Success(
                MapToDto(refundTransaction),
                "Refund transaction created successfully",
                201);
        }

        /// <summary>
        /// PURCHASE transaction işlemi - İş mantığı
        /// Bakiyeyi arttır
        /// </summary>
        private async Task<Result<TransactionDto>> ProcessPurchaseTransactionAsync(
            CreateTransactionRequest request,
            Account account)
        {
            // ===== İŞ MANTTIĞI: BAKIYE GÜNCELLEME =====
            
            account.Balance += request.Amount;

            // ===== İŞ MANTTIĞI: TRANSACTION OLUŞTURMA =====
            
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                Amount = request.Amount,
                Currency = request.Currency,
                TransactionType = request.TransactionType,
                Description = request.Description,
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.Success,
                ReferenceTransactionId = null
            };

            // ===== İŞ MANTTIĞI: VERITABANINA KAYDET =====
            
            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.SaveChangesAsync();

            return Result<TransactionDto>.Success(
                MapToDto(transaction),
                "Purchase transaction created successfully",
                201);
        }

        /// <summary>
        /// Tüm transaction'ları getir - Basit veri erişimi
        /// </summary>
        public async Task<Result<IEnumerable<TransactionDto>>> GetAllTransactionsAsync()
        {
            // ===== Basit veri erişimi - İş mantığı yok =====
            
            var transactions = await _transactionRepository.GetAllAsync();
            var dtos = transactions.Select(MapToDto).ToList();

            return Result<IEnumerable<TransactionDto>>.Success(dtos);
        }

        /// <summary>
        /// Belirli account'a ait transaction'ları getir
        /// </summary>
        public async Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByAccountIdAsync(Guid accountId)
        {
            // ===== İŞ MANTTIĞI: VALIDASYON =====
            
            if (accountId == Guid.Empty)
                return Result<IEnumerable<TransactionDto>>.Failure(
                    "Invalid account ID",
                    400);

            // ===== Account var mı kontrol et =====
            
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return Result<IEnumerable<TransactionDto>>.Failure(
                    "Account not found",
                    404);

            // ===== Veri erişimi =====
            
            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
            var dtos = transactions.Select(MapToDto).ToList();

            return Result<IEnumerable<TransactionDto>>.Success(dtos);
        }

        /// <summary>
        /// Belirli tarih aralığında transaction'ları getir
        /// </summary>
        public async Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            // ===== İŞ MANTTIĞI: VALIDASYON =====
            
            if (startDate > endDate)
                return Result<IEnumerable<TransactionDto>>.Failure(
                    "Start date cannot be greater than end date",
                    400);

            // ===== Veri erişimi =====
            
            var transactions = await _transactionRepository.GetByDateRangeAsync(startDate, endDate);
            var dtos = transactions.Select(MapToDto).ToList();

            return Result<IEnumerable<TransactionDto>>.Success(dtos);
        }

        /// <summary>
        /// Belirli transaction tipine göre transaction'ları getir
        /// </summary>
        public async Task<Result<IEnumerable<TransactionDto>>> GetTransactionsByTypeAsync(string transactionType)
        {
            // ===== İŞ MANTTIĞI: VALIDASYON =====
            
            if (string.IsNullOrWhiteSpace(transactionType))
                return Result<IEnumerable<TransactionDto>>.Failure(
                    "Transaction type cannot be empty",
                    400);

            // ===== Enum dönüşümü kontrolü =====
            
            if (!Enum.TryParse<TransactionType>(transactionType, ignoreCase: true, out _))
                return Result<IEnumerable<TransactionDto>>.Failure(
                    $"Invalid transaction type: {transactionType}",
                    400);

            // ===== Veri erişimi =====
            
            var transactions = await _transactionRepository.GetByTransactionTypeAsync(transactionType);
            var dtos = transactions.Select(MapToDto).ToList();

            return Result<IEnumerable<TransactionDto>>.Success(dtos);
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Helper: Entity'yi DTO'ya dönüştür
        /// </summary>
        private TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                TransactionStatus = transaction.Status.ToString(),
                TransactionType = transaction.TransactionType.ToString(),
                Date = transaction.TransactionDate,
                Description = transaction.Description,
                Currency = transaction.Currency.ToString(),
                Children = transaction.ChildTransactions?
                    .Select(MapToDto)
                    .ToList() ?? new List<TransactionDto>()
            };
        }
    }
}