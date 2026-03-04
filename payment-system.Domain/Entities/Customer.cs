using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;

namespace payment_system.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        //navigation property (one-to-many) for accounts 
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}