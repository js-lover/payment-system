namespace payment_system.Api.DTOs
{
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public List<TransactionDto> Transactions { get; set; }

    }
}