using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;
using payment_system.Domain.Enums;

namespace payment_system.Domain.Entities
{
    public class Card : BaseEntity
    {
        //card number must be unique and masked like "1234 **** **** 6789"
        public string CardNumber { get; set; } = null!;
        public string CardName { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        //cvv cannot be stored in plain text and must be encrypted or never save to db
        public string CVC { get; set; } = null!;
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }
        public CardStatus Status { get; set; }

        //a card can have multiple transactions
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
} 