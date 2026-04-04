using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Customer;
using payment_system.Application.DTOs.Account;

namespace payment_system.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;

        public CustomerService(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }


       
        /// <summary>
        /// Maps a Customer entity and its associated accounts to a CustomerDto.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private CustomerDto MapToCustomerDetailsDto(Customer customer, IEnumerable<Account> accounts)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Surname = customer.Surname,
                Email = customer.Email,
                NationalId = customer.NationalId,
                Accounts = accounts.Select(a => new AccountDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    Name = a.Name,
                    Balance = a.Balance,
                    Currency = a.Currency.ToString()
                }).ToList()
            };
        }

        //=======READ Operations======

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<CustomerDto>> GetCustomerByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result<CustomerDto>.Failure("Invalid customer ID.");

            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
                return Result<CustomerDto>.Failure("Customer not found.");

            var accounts = await _accountRepository.GetAllByCustomerIdAsync(id);
            var result = MapToCustomerDetailsDto(customer, accounts);

            return Result<CustomerDto>.Success(result);
        }


        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<CustomerDto>>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            var customerDtos = new List<CustomerDto>();

            foreach (var customer in customers)
            {
                var accounts = await _accountRepository.GetAllByCustomerIdAsync(customer.Id);
                customerDtos.Add(MapToCustomerDetailsDto(customer, accounts));
            }

            return Result<IEnumerable<CustomerDto>>.Success(customerDtos);
        }


        //=======CREATE Operations======
        public async Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            if (request == null)
            {
                return Result<CustomerDto>.Failure("Invalid customer data.");
            }

            var existingCustomerByNationalId = await _customerRepository.GetCustomerByNationalIdAsync(request.NationalId);
            if (existingCustomerByNationalId != null)
            {
                return Result<CustomerDto>.Failure("A customer with the same National ID already exists.");
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PasswordHash = request.PasswordHash, // In a real application, you should hash the password before storing it
                NationalId = request.NationalId,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow

            };

            await _customerRepository.CreateCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            var result = MapToCustomerDetailsDto(customer , new List<Account>());
            return Result<CustomerDto>.Success(result);
        }




        //=========DELETE Operations========

        public async Task<Result<CustomerDto>> DeleteCustomerAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result<CustomerDto>.Failure("Invalid customer ID.");

            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
                return Result<CustomerDto>.Failure("Customer not found.");

            await _customerRepository.DeleteCustomerAsync(id);
            await _customerRepository.SaveChangesAsync();

            var result = MapToCustomerDetailsDto(customer, new List<Account>());
            return Result<CustomerDto>.Success(result);
        }

    }
}
