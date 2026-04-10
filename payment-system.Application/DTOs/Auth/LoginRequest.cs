// for email and password

namespace payment_system.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}