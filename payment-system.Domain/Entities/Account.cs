using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;
using payment_system.Domain.Enums;

namespace payment_system.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string AccountNumber { get; set; }
        //not double or float because we want to avoid precision issues with large numbers
        public decimal Balance { get; set; }
        //using enum for currency
        public Currency Currency { get; set; }

        //customer entity foreign key 
        public Guid CustomerId { get; set; }
        //navigation property from customer
        public virtual Customer Customer { get; set; }
        //an account can have multiple cards
        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

        //an account can have multiple transactions
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}