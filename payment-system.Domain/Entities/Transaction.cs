using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;
using payment_system.Domain.Enums;


namespace payment_system.Domain.Entities

{
    public class Transaction: BaseEntity
    {
        //the reason we use a separate class for transactions is to keep track of all payment activities
        //account id is used to identify the user account associated with the transaction
        public Guid AccountId { get; set; }
        //the reason we use a nullable Guid for CardId is to account for transactions that may not involve a card
        public Guid? CardId { get; set; } 
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus Status { get; set; }
        public string? Description { get; set; }
        /// Reference to another transaction, if this transaction is related to one.
        public Guid? ReferenceTransactionId { get; set; }

    }
}