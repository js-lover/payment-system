using System;
using System.Collections.Generic;

namespace payment_system.Application.DTOs.Transaction
{
    //DTO for Transaction
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionStatus { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;

        //self-referencing 
        public List<TransactionDto> Children { get; set; } = new();
    }

}