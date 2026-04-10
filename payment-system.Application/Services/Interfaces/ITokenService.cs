using payment_system.Domain.Entities;

namespace payment_system.Application.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the specified email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        string CreateToken(User user);
    }
}
