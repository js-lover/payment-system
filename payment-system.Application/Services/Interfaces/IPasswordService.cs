namespace payment_system.Application.Services.Interfaces
{
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes the provided password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies the provided password against the hashed password.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}