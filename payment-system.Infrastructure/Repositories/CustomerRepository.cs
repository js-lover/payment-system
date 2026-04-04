using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Infrastructure.Persistence.Contexts;

namespace payment_system.Infrastructure.Repositories
{
    /// <summary>
    /// Customer repository implementasyonu
    /// Sadece Customer operasyonlarını gerçekleştirir
    /// İş mantığı yoktur - sadece veri erişimi
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {      

        private readonly AppDbContext _db;

        public CustomerRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        //============== CREATE Operations ==============

        ///<summary>
        /// Create Customer
        /// </summary>
        /// <param name="customer"></param>
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            await _db.Customers.AddAsync(customer);
            return customer;
        }

        //============== READ Operations ==============

        /// <summary>
        /// Get Customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Customer> GetCustomerByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid customer ID.", nameof(id));

            return await _db.Customers.FindAsync(id);
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _db.Customers.ToListAsync();
        }

         
        /// <summary>
        /// Get a customer by National ID
        /// </summary>
        /// <param name="nationalId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Customer> GetCustomerByNationalIdAsync(string nationalId)
        {
            if (nationalId == null)
                throw new ArgumentException("Invalid National ID.", nameof(nationalId));

            return await _db.Customers.FirstOrDefaultAsync(x => x.NationalId == nationalId);
        }


        //============DELETE OPERATIONS =============

        public async Task DeleteCustomerAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid customer ID.", nameof(id));
            
            var customer = await _db.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.IsDeleted = true; // Soft delete
            }
        }




        //========== SAVE Operations ==========

        /// <summary>
        /// Save changes to the database
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

      
    }
}