namespace payment_system.Application.DTOs.Account
{
    public class CreateAccountRequest
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public decimal InitialBalance { get; set; } = 0;
    }
}