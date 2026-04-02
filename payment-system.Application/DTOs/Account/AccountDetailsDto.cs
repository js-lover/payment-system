using payment_system.Application.DTOs.Transaction;

namespace payment_system.Application.DTOs.Account
{
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }  // ✅ Eklendi
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;  // ✅ Eklendi
        public decimal Balance { get; set; }
        public string Currency { get; set; } = null!;  // Enum → String
        public List<TransactionDto> Transactions { get; set; } = new();
    }
}