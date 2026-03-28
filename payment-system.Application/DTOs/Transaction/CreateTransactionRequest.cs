using payment_system.Domain.Enums;


namespace payment_system.Application.DTOs.Transaction
{
    public class CreateTransactionRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Domain.Enums.TransactionType TransactionType { get; set; }
        public Domain.Enums.Currency Currency { get; set; }
        public Guid? ReferenceTransactionId { get; set; }
    }
}
