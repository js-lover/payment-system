namespace payment_system.Application.DTOs.Transaction
{
    //DTO for Transaction
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime Date { get; set; }
        public string TransactionStatus { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public string Description { get; set; } = null!;

        //self-referencing 
        public List<TransactionDto> Children { get; set; } = new();
    }

}