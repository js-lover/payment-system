using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Entities;
using payment_system.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using payment_system.Application.Repositories;

namespace payment_system.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }


        /// <summary>
        /// Get a user by email address (case-insensitive).
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>User object or null</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            // Case-insensitive email comparison
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Create a new user (add to User table).
        /// </summary>
        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _db.Users.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Save all changes to database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Get user by ID.
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        /// <summary>
        /// Get all users with Admin role (with customer profile if exists).
        /// </summary>
        public async Task<List<User>> GetAllAdminsByRoleAsync()
        {
            return await _db.Users
                .Where(u => u.Role == payment_system.Domain.Enums.UserRole.Admin && !u.IsDeleted)
                .Include(u => u.Customer)
                .ToListAsync();
        }
    }
}