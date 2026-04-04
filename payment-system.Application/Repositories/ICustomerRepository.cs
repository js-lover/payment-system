using payment_system.Domain.Entities;

namespace payment_system.Application.Repositories
{
    public interface ICustomerRepository
    {   

        //==========CREATE Operations==========

        ///<summary>
        /// Create Customer
        /// </summary>
        Task<Customer> CreateCustomerAsync(Customer customer);


        //==========READ Operations==========

        ///<summary>
        /// Get Customer by Id
        /// </summary>
        Task<Customer> GetCustomerByIdAsync(Guid id);

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Customer>> GetAllCustomersAsync();


        ///<summary>
        /// Get Customer by NationalId
        /// </summary>
        Task<Customer> GetCustomerByNationalIdAsync(string nationalId);
        

        //======= DELETE Operations==========

        /// <summary>
        /// Delete Customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteCustomerAsync(Guid id);






        //========== SAVE Operations==========

        /// <summary>
        /// Save all changes
        /// </summary>
        Task SaveChangesAsync();
    }
}