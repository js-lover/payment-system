using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;

namespace payment_system.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public virtual User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string NationalId { get; set; } = null!;
        //navigation property (one-to-many) for accounts 
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}