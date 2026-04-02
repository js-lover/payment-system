namespace payment_system.Application.DTOs.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = null!;  // Enum → String
    }
}