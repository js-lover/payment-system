using Xunit;
using Moq;
using payment_system.Application.Services.Implementations;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Repositories;
using payment_system.Application.DTOs.Customer;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_system.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockPasswordService = new Mock<IPasswordService>();

            _customerService = new CustomerService(
                _mockCustomerRepository.Object,
                _mockAccountRepository.Object,
                _mockPasswordService.Object,
                _mockUserRepository.Object
            );
        }

        #region CreateCustomerAsync Tests

        /// <summary>
        /// Test successful customer creation with valid data.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Ahmet",
                Surname = "Yılmaz",
                Email = "ahmet@example.com",
                PasswordHash = "SecurePassword123!",
                NationalId = "12345678901",
                PhoneNumber = "+905551234567",
                DateOfBirth = new DateTime(1990, 1, 15)
            };

            var hashedPassword = "$2a$11$hashedpassword";
            _mockPasswordService.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns(hashedPassword);

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _mockCustomerRepository.Setup(x => x.GetCustomerByNationalIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Customer)null);

            _mockCustomerRepository.Setup(x => x.CreateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) => customer);

            _mockCustomerRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("ahmet@example.com", result.Data.Email);
            Assert.Equal("Ahmet", result.Data.Name);
            Assert.Equal("Yılmaz", result.Data.Surname);
            Assert.Equal("12345678901", result.Data.NationalId);

            // Verify that repositories were called
            _mockUserRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockCustomerRepository.Verify(x => x.GetCustomerByNationalIdAsync(It.IsAny<string>()), Times.Once);
            _mockCustomerRepository.Verify(x => x.CreateCustomerAsync(It.IsAny<Customer>()), Times.Once);
            _mockCustomerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockPasswordService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test customer creation fails when email is already registered.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_EmailExists_ReturnsFail()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Mehmet",
                Surname = "Demir",
                Email = "existing@example.com",
                PasswordHash = "Password123!",
                NationalId = "98765432109",
                PhoneNumber = "+905559876543",
                DateOfBirth = new DateTime(1995, 5, 20)
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "existing@example.com",
                PasswordHash = "$2a$11$hashedpassword",
                Role = UserRole.Customer
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("email adresi zaten kayıtlı", result.Message);

            // Verify CreateCustomer was NOT called
            _mockCustomerRepository.Verify(x => x.CreateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        /// <summary>
        /// Test customer creation fails when national ID is already registered.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_NationalIdExists_ReturnsFail()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Ayşe",
                Surname = "Kara",
                Email = "ayse@example.com",
                PasswordHash = "Password123!",
                NationalId = "12345678901",
                PhoneNumber = "+905551111111",
                DateOfBirth = new DateTime(1992, 3, 10)
            };

            var existingCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Existing",
                Surname = "Customer",
                NationalId = "12345678901",
                PhoneNumber = "+905552222222",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _mockCustomerRepository.Setup(x => x.GetCustomerByNationalIdAsync(It.IsAny<string>()))
                .ReturnsAsync(existingCustomer);

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("TC Kimlik Numarası", result.Message);

            // Verify CreateCustomer was NOT called
            _mockCustomerRepository.Verify(x => x.CreateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        /// <summary>
        /// Test customer creation fails with null request.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_NullRequest_ReturnsFail()
        {
            // Act
            var result = await _customerService.CreateCustomerAsync(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Geçersiz", result.Message);
        }

        /// <summary>
        /// Test customer creation fails when email is empty.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_EmptyEmail_ReturnsFail()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Ali",
                Surname = "Veli",
                Email = "",
                PasswordHash = "Password123!",
                NationalId = "11111111111",
                PhoneNumber = "+905553333333",
                DateOfBirth = new DateTime(1991, 7, 15)
            };

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Email", result.Message);
        }

        /// <summary>
        /// Test customer creation fails when password is empty.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_EmptyPassword_ReturnsFail()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Zeynep",
                Surname = "Erol",
                Email = "zeynep@example.com",
                PasswordHash = "",
                NationalId = "22222222222",
                PhoneNumber = "+905554444444",
                DateOfBirth = new DateTime(1993, 9, 20)
            };

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Şifre", result.Message);
        }

        /// <summary>
        /// Test email comparison is case-insensitive.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_EmailCaseInsensitive_ReturnsFail()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Test",
                Surname = "User",
                Email = "TEST@EXAMPLE.COM", // Büyük harf
                PasswordHash = "Password123!",
                NationalId = "33333333333",
                PhoneNumber = "+905555555555",
                DateOfBirth = new DateTime(1994, 11, 25)
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com", // Küçük harf
                PasswordHash = "$2a$11$hashedpassword",
                Role = UserRole.Customer
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync("TEST@EXAMPLE.COM"))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("email adresi zaten kayıtlı", result.Message);
        }

        /// <summary>
        /// Test password hashing is verified correctly.
        /// </summary>
        [Fact]
        public async Task CreateCustomerAsync_PasswordHashingVerified()
        {
            // Arrange
            var request = new CreateCustomerRequest
            {
                Name = "Test",
                Surname = "User",
                Email = "test@example.com",
                PasswordHash = "PlainTextPassword123!",
                NationalId = "44444444444",
                PhoneNumber = "+905556666666",
                DateOfBirth = new DateTime(1996, 2, 14)
            };

            var hashedPassword = "$2a$11$differenthash";
            _mockPasswordService.Setup(x => x.HashPassword("PlainTextPassword123!"))
                .Returns(hashedPassword);

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _mockCustomerRepository.Setup(x => x.GetCustomerByNationalIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Customer)null);

            _mockCustomerRepository.Setup(x => x.CreateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) => customer);

            _mockCustomerRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerService.CreateCustomerAsync(request);

            // Assert
            _mockPasswordService.Verify(
                x => x.HashPassword("PlainTextPassword123!"),
                Times.Once,
                "Password should be hashed with BCrypt"
            );
        }

        #endregion

        #region GetCustomerByIdAsync Tests

        /// <summary>
        /// Test get customer by ID returns success with valid ID.
        /// </summary>
        [Fact]
        public async Task GetCustomerByIdAsync_ValidId_ReturnsSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PasswordHash = "$2a$11$hashedpassword",
                Role = UserRole.Customer
            };

            var customer = new Customer
            {
                Id = customerId,
                UserId = userId,
                User = user,
                Name = "Test",
                Surname = "User",
                NationalId = "12345678901",
                PhoneNumber = "+905551234567",
                DateOfBirth = new DateTime(1990, 1, 15)
            };

            _mockCustomerRepository.Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(customerId))
                .ReturnsAsync(new List<Account>());

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("test@example.com", result.Data.Email);
        }

        /// <summary>
        /// Test get customer by ID fails with invalid ID.
        /// </summary>
        [Fact]
        public async Task GetCustomerByIdAsync_InvalidId_ReturnsFail()
        {
            // Act
            var result = await _customerService.GetCustomerByIdAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid", result.Message);
        }

        #endregion

        #region GetAllCustomersAsync Tests

        /// <summary>
        /// Test get all customers returns all customer records.
        /// </summary>
        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), Name = "Customer1", Surname = "Surname1", NationalId = "111", User = new User { Email = "customer1@example.com" } },
                new Customer { Id = Guid.NewGuid(), Name = "Customer2", Surname = "Surname2", NationalId = "222", User = new User { Email = "customer2@example.com" } }
            };

            _mockCustomerRepository.Setup(x => x.GetAllCustomersAsync())
                .ReturnsAsync(customers);

            _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Account>());

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, ((List<CustomerDto>)result.Data).Count);
        }

        #endregion
    }
}
