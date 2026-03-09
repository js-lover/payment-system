namespace payment_system.Api.DTOs
{
    //DTO for Transaction
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }

        //self-referencing 
        public List<TransactionDto> Children { get; set; } = new();
    }

}