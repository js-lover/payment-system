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
using payment_system.Domain.Enums;

namespace payment_system.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;

        public CustomerService(
            ICustomerRepository customerRepository, 
            IAccountRepository accountRepository, 
            IPasswordService passwordService,
            IUserRepository userRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _passwordService = passwordService;
            _userRepository = userRepository;
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
                Email = customer.User.Email,
                NationalId = customer.NationalId,
                PhoneNumber = customer.PhoneNumber,
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

        /// <summary>
        /// Müşteri profili ve kullanıcı kimliğini oluşturur.
        /// 
        /// İş Akışı:
        /// 1. İstek ve email doğrulaması
        /// 2. Kullanıcı (Identity) oluşturma - Email ve şifre ile
        /// 3. Müşteri (Profile) oluşturma - Profil bilgileri ile
        /// 4. Atomik işlem - SaveChanges ile kalıcı hale getirme
        /// 
        /// Bu tasarım "User-First" mimarisini uygulamaktadır:
        /// - Her müşteri bir kullanıcı (Identity)
        /// - Ancak her kullanıcı (örneğin Admin) bir müşteri olmak zorunda değil
        /// </summary>
        /// <param name="request">Müşteri oluşturma isteği</param>
        /// <returns>Oluşturulan müşteri bilgilerini içeren DTO</returns>
        public async Task<Result<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                // ========== VALIDASYON ==========
                
                // Null kontrolü
                if (request == null)
                {
                    return Result<CustomerDto>.Failure("Geçersiz müşteri verisi.");
                }

                // Email alanı kontrolü
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Result<CustomerDto>.Failure("Email adresi boş olamaz.");
                }

                if (string.IsNullOrWhiteSpace(request.PasswordHash))
                {
                    return Result<CustomerDto>.Failure("Şifre boş olamaz.");
                }

                // Ad ve Soyadı kontrolü
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
                {
                    return Result<CustomerDto>.Failure("Ad ve soyad boş olamaz.");
                }

                // TC Kimlik No kontrolü
                if (string.IsNullOrWhiteSpace(request.NationalId))
                {
                    return Result<CustomerDto>.Failure("TC Kimlik Numarası boş olamaz.");
                }

                // ADIM 1: Identity (User) Tablosunda Email Kontrolü
                // Bu krit kontrol - eğer bu email'de zaten bir kullanıcı varsa işlem başarısız
                var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return Result<CustomerDto>.Failure("Bu email adresi zaten kayıtlı. Lütfen başka bir email kullanın.");
                }

                // ADIM 2: Profil (Customer) Tablosunda NationalId Kontrolü
                var existingCustomerByNationalId = await _customerRepository.GetCustomerByNationalIdAsync(request.NationalId);
                if (existingCustomerByNationalId != null)
                {
                    return Result<CustomerDto>.Failure("Bu TC Kimlik Numarası ile zaten bir müşteri kayıtlı.");
                }

                // ========== USER (IDENTITY) OLUŞTURMA ==========
                
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim(),
                    // BCrypt ile hash işlemi - hassas şifre asla düz metin olarak saklanmaz
                    PasswordHash = _passwordService.HashPassword(request.PasswordHash),
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                };

                // ========== CUSTOMER (PROFILE) OLUŞTURMA ==========
                
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    // Bire-bir ilişki - Customer UserId ile User'ı referans alır
                    UserId = user.Id,
                    User = user,
                    // Profil bilgileri
                    Name = request.Name.Trim(),
                    Surname = request.Surname.Trim(),
                    NationalId = request.NationalId.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow
                };

                // ========== KALICI VERİ (PERSISTENCE) ==========
                
                // CreateCustomerAsync metodu içinde User'ı Navigation Property üzerinden
                // otomatik olarak veritabanına eklemelidir
                await _customerRepository.CreateCustomerAsync(customer);
                
                // Atomik işlem - tüm değişiklikleri bir kez kaydet
                await _customerRepository.SaveChangesAsync();

                // ========== ÇIKTI VE DTO DÖNÜŞÜMÜ ==========
                
                var resultDto = MapToCustomerDetailsDto(customer, new List<Account>());
                
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

            var result = MapToCustomerDetailsDto(customer, new List<Account>());
            return Result<CustomerDto>.Success(result);
        }

    }
}
