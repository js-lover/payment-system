using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using payment_system.Application.Repositories;
using payment_system.Domain.Entities;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Common;
using payment_system.Application.DTOs.Customer;
using payment_system.Application.DTOs.Account;
using payment_system.Domain.Enums;

namespace payment_system.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository customerRepository, 
            IAccountRepository accountRepository, 
            IPasswordService passwordService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _passwordService = passwordService;
            _userRepository = userRepository;
            _mapper = mapper;
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

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Result<CustomerDto>.Success(customerDto);
        }


        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<CustomerDto>>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Result<IEnumerable<CustomerDto>>.Success(customerDtos);
        }


        //=======CREATE Operations======

        /// <summary>
        /// Create a customer profile and user identity.
        /// 
        /// Workflow:
        /// 1. Validate request and email
        /// 2. Create user (Identity) with email and password
        /// 3. Create customer (Profile) with customer details
        /// 4. Save changes atomically
        /// 
        /// Design: "User-First" architecture
        /// - Each customer is a user (Identity)
        /// - But not every user (e.g., Admin) needs to be a customer
        /// </summary>
        /// <param name="request">Customer creation request</param>
        /// <returns>Created customer details in DTO</returns>
        public async Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                // ========== VALIDATION ==========
                
                // Check for null
                if (request == null)
                {
                    return Result<CustomerDto>.Failure("Invalid customer data.");
                }

                // Check email field
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Result<CustomerDto>.Failure("Email cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(request.PasswordHash))
                {
                    return Result<CustomerDto>.Failure("Password cannot be empty.");
                }

                // Check name and surname
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
                {
                    return Result<CustomerDto>.Failure("Name and surname cannot be empty.");
                }

                // Check national ID
                if (string.IsNullOrWhiteSpace(request.NationalId))
                {
                    return Result<CustomerDto>.Failure("National ID cannot be empty.");
                }

                // STEP 1: Check email in User table
                // Critical check - if email exists, operation fails
                var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return Result<CustomerDto>.Failure("This email address is already registered. Please use another email.");
                }

                // STEP 2: Check national ID in Customer table
                var existingCustomerByNationalId = await _customerRepository.GetCustomerByNationalIdAsync(request.NationalId);
                if (existingCustomerByNationalId != null)
                {
                    return Result<CustomerDto>.Failure("A customer with this national ID is already registered.");
                }

                // ========== CREATE USER (IDENTITY) ==========
                
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim(),
                    // Hash password with BCrypt - sensitive password never stored as plain text
                    PasswordHash = _passwordService.HashPassword(request.PasswordHash),
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                };

                // ========== CREATE CUSTOMER (PROFILE) ==========
                
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    // One-to-one relationship - Customer references User via UserId
                    UserId = user.Id,
                    User = user,
                    // Profile information
                    Name = request.Name.Trim(),
                    Surname = request.Surname.Trim(),
                    NationalId = request.NationalId.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow
                };

                // ========== PERSISTENCE ==========
                
                // CreateCustomerAsync should automatically add User to database
                // via Navigation Property
                await _customerRepository.CreateCustomerAsync(customer);
                
                // Atomic operation - save all changes at once
                await _customerRepository.SaveChangesAsync();

                // ========== OUTPUT AND DTO CONVERSION ==========
                
                var resultDto = _mapper.Map<CustomerDto>(customer);
                
                return Result<CustomerDto>.Success(
                    resultDto, 
                    "Müşteri başarıyla oluşturuldu.", 
                    201
                );
            }
            catch (Exception ex)
            {
                // Genel hata yakalama - hassas verileri loglama
                return Result<CustomerDto>.Failure(
                    $"Müşteri oluşturulurken bir hata oluştu: {ex.Message}",
                    500
                );
            }
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

            var result = _mapper.Map<CustomerDto>(customer);
            return Result<CustomerDto>.Success(result);
        }

    }
}
