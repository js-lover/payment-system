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
        /// Retrieves a user by their email address (case-insensitive).
        /// </summary>
        /// <param name="email">Email adresi</param>
        /// <returns>User nesnesi veya null</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            // Case-insensitive email karşılaştırması
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Yeni kullanıcı oluştur (User tablosuna ekle)
        /// </summary>
        public async Task<User> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _db.Users.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydet
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// ID'ye göre kullanıcı getir
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Tüm kullanıcıları getir
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        /// <summary>
        /// Tüm Admin role'deki kullanıcıları getir (Customer profili ile birlikte)
        /// </summary>
        public async Task<List<User>> GetAllAdminsByRoleAsync()
        {
            return await _db.Users
                .Where(u => u.Role == payment_system.Domain.Enums.UserRole.Admin && !u.IsDeleted)
                .Include(u => u.Customer)  // Customer profili varsa include et
                .ToListAsync();
        }
    }
}