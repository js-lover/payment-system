using System;
using payment_system.Domain.Enums;
using payment_system.Domain.Common;
namespace payment_system.Domain.Entities
{
    public class User : BaseEntity
    {
        public UserRole Role { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public virtual Customer? Customer { get; set; }
            
    }
}