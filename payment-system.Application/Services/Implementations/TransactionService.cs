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
    /// Tüm transaction operasyonlarını gerçekleştirir
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        /// <summary>
        /// Constructor - Dependency Injection aracılığıyla Repository injekte edilir
        /// </summary>
        /// <param name="repository">Transaction repository</param>
        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Yeni transaction oluştur - validasyonlar yapıldıktan sonra
        /// </summary>
        public async Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request)
        {
            // ===== STEP 1: TEMEL VALIDASYONLAR =====
            
            // Amount kontrolü
            if (request.Amount <= 0)
                return Result<TransactionDto>.Failure(
                    "Amount must be greater than zero.", 
                    400);

            // Account var mı kontrolü
            var account = await _repository.GetAccountByIdAsync(request.AccountId);
            if (account == null)
                return Result<TransactionDto>.Failure(
                    "Account not found.", 
                    404);

            // ===== STEP 2: İŞLEM TİPİNE GÖRE VALIDASYON =====
            
            // REFUND işleminin validasyonu
            if (request.TransactionType == TransactionType.Refund)
            {
                var (refundValidation, referenceTransaction) = 
                    await ValidateRefundTransactionAsync(request, account);
                
                if (!refundValidation.IsSuccess)
                    return refundValidation;
            }

            // SALE işleminin validasyonu ve bakiye güncelleme
            if (request.TransactionType == TransactionType.Sale)
            {
                if (account.Balance < request.Amount)
                    return Result<TransactionDto>.Failure(
                        "Insufficient balance for sale.", 
                        400);

                // Bakiyeyi düş
                account.Balance -= request.Amount;
            }

            // ===== STEP 3: TRANSACTION OLUŞTUR =====
            var transaction = new Transaction
            {
                AccountId = request.AccountId,
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Description = request.Description,
                Currency = request.Currency,
                TransactionType = request.TransactionType,
                ReferenceTransactionId = request.ReferenceTransactionId,
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.Success
            };

            // ===== STEP 4: VERITABANINA KAYDET =====
            await _repository.AddTransactionAsync(transaction);
            await _repository.SaveChangesAsync();

            // ===== STEP 5: RESPONSE DTO'SU OLUŞTUR =====
            var dto = MapToDto(transaction);
            
            return Result<TransactionDto>.Success(
                dto, 
                "Transaction created successfully", 
                201);
        }

        /// <summary>
        /// Tüm transaction'ları parent-child ilişkisi ile getir
        /// </summary>
        public async Task<Result<IEnumerable<TransactionDto>>> GetAllTransactionsAsync()
        {
            var transactions = await _repository.GetAllTransactionsAsync();
            var result = transactions.Select(MapToDto).ToList();
            
            return Result<IEnumerable<TransactionDto>>.Success(result);
        }

        /// <summary>
        /// REFUND transaction'ının tüm validasyonlarını yap
        /// Tuple döndürür: (ValidationResult, ReferenceTransaction)
        /// </summary>
        private async Task<(Result<TransactionDto>, Transaction?)> ValidateRefundTransactionAsync(
            CreateTransactionRequest request, 
            Account account)
        {
            // Reference transaction ID gerekli mi?
            if (!request.ReferenceTransactionId.HasValue)
                return (Result<TransactionDto>.Failure(
                    "Refund transactions must reference an existing transaction.",
                    400), null);

            // Reference transaction var mı?
            var referenceTransaction = await _repository.GetTransactionByIdAsync(
                request.ReferenceTransactionId.Value);

            if (referenceTransaction == null)
                return (Result<TransactionDto>.Failure(
                    "Invalid reference transaction.",
                    400), null);

            // Reference transaction aynı account'a mı ait?
            if (referenceTransaction.AccountId != request.AccountId)
                return (Result<TransactionDto>.Failure(
                    "Cannot refund a transaction from another account.",
                    400), null);

            // Reference transaction SALE mi?
            if (referenceTransaction.TransactionType != TransactionType.Sale)
                return (Result<TransactionDto>.Failure(
                    "Only sales transactions can be refunded.",
                    400), null);

            // Toplam refund amount orijinal amount'ı geçmiş mi?
            var totalRefundAmount = referenceTransaction.ChildTransactions?.Sum(x => x.Amount) ?? 0;
            if (totalRefundAmount + request.Amount > referenceTransaction.Amount)
                return (Result<TransactionDto>.Failure(
                    "Refund amount exceeds original transaction amount.",
                    400), null);

            // Bakiyeyi geri yükle
            account.Balance += request.Amount;
            
            return (Result<TransactionDto>.Success(null!), referenceTransaction);
        }

        /// <summary>
        /// Transaction entity'sini DTO'ya dönüştür
        /// Recursive olarak child transaction'ları da dönüştürür
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