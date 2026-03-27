using payment_system.Application.DTOs.Transaction;

namespace payment_system.Application.DTOs.Account
{
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = null!;
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();

    }
}