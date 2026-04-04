using System;
using payment_system.Application.DTOs.Account;

namespace payment_system.Application.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<AccountDto> Accounts { get; set; } = new();
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
    }
}