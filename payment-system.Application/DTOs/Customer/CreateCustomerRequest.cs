using payment_system.Domain.Enums;

namespace payment_system.Application.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
    }
}