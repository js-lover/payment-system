// for token and expire time

namespace payment_system.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireTime { get; set; }
        public string Email { get; set; } = null!;
    }
}