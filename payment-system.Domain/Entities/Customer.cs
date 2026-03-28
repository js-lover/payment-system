using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Common;

namespace payment_system.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        //navigation property (one-to-many) for accounts 
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}