using payment_system.Domain.Enums;

namespace payment_system.Api.DTOs
{
public class CreateTransactionRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount {get; set;}
        public string? Description {get; set;}
        public Currency Currency {get; set;}
        public TransactionType TransactionType {get; set;}
        public Guid? ReferenceTransactionId {get; set;}
    }
}
