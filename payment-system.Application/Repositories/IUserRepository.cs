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
        /// Yeni kullanıcı oluştur (User tablosuna ekle)
        /// </summary>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydet
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// ID'ye göre kullanıcı getir
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Tüm kullanıcıları getir
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Tüm Admin role'deki kullanıcıları getir (Customer profili ile birlikte)
        /// </summary>
        Task<List<User>> GetAllAdminsByRoleAsync();
    }
}