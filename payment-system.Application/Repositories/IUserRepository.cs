using payment_system.Domain.Entities;

namespace payment_system.Application.Services.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Create a new user (add to User table).
        /// </summary>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Get user by ID.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all users.
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Get all users with Admin role (with customer profile).
        /// </summary>
        Task<List<User>> GetAllAdminsByRoleAsync();
    }
}