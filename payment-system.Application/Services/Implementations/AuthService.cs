using payment_system.Application.Common;
using payment_system.Application.DTOs.Auth;
using payment_system.Application.DTOs.Admin;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;
using payment_system.Application.DTOs.Customer;

namespace payment_system.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ICustomerRepository _customerRepository;

        public AuthService(
            IUserRepository userRepository, 
            IPasswordService passwordService, 
            ITokenService tokenService,
            ICustomerRepository customerRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _customerRepository = customerRepository;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            // 1. Find user by email in database
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // 2. Return error if user not found or is deleted
            // Security Note: Returning generic "invalid credentials" message is more secure than revealing email existence
            if (user == null)
            {
                return Result<AuthResponse>.Failure("Invalid email or password.");
            }

            // 3. Verify password using BCrypt hash comparison
            var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Result<AuthResponse>.Failure("Invalid email or password.");
            }

            // 4. Authentication successful, generate JWT token
            var token = _tokenService.CreateToken(user);

            // 5. Package successful result and return
            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                ExpireTime = DateTime.UtcNow.AddMinutes(60) // Must align with token expiration time
        
            };

            return Result<AuthResponse>.Success(response);
        }

        /// <summary>
        /// Registers a new user account with email, password, and role.
        /// 
        /// User-First Architecture:
        /// 1. Create User (Identity) with Email, Password, Role
        /// 2. If Customer role, create Customer (Profile)
        /// 3. For Admin/Staff roles, only User is created (no Customer profile)
        /// 4. Generate and return JWT token
        /// </summary>
        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // ========== STEP 1: VALIDATION ==========

                if (request == null)
                    return Result<AuthResponse>.Failure("Invalid registration request.");

                if (string.IsNullOrWhiteSpace(request.Email))
                    return Result<AuthResponse>.Failure("Email address cannot be empty.");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return Result<AuthResponse>.Failure("Password cannot be empty.");

                if (request.Password != request.ConfirmPassword)
                    return Result<AuthResponse>.Failure("Passwords do not match.");

                if (request.Password.Length < 6)
                    return Result<AuthResponse>.Failure("Password must be at least 6 characters long.");

                // Is email already registered?
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                    return Result<AuthResponse>.Failure("This email address is already registered.");

                // ========== STEP 2: CREATE USER (IDENTITY) ==========

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim(),
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    Role = request.Role,  // Get role from request (Customer, Admin, Staff)
                    CreatedAt = DateTime.UtcNow
                };

                // ========== STEP 3: CREATE CUSTOMER PROFILE (Customer role only) ==========

                if (request.Role == UserRole.Customer)
                {
                    // Create customer profile
                    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
                        return Result<AuthResponse>.Failure("First name and last name cannot be empty (required for Customer).");

                    if (string.IsNullOrWhiteSpace(request.NationalId))
                        return Result<AuthResponse>.Failure("National ID cannot be empty (required for Customer).");

                    // Verify National ID is not already registered
                    var existingNationalId = await _customerRepository.GetCustomerByNationalIdAsync(request.NationalId);
                    if (existingNationalId != null)
                        return Result<AuthResponse>.Failure("This National ID is already registered.");

                    // Create customer
                    var customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        User = user,
                        Name = request.Name.Trim(),
                        Surname = request.Surname.Trim(),
                        NationalId = request.NationalId.Trim(),
                        PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
                        DateOfBirth = request.DateOfBirth ?? DateTime.MinValue,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Save customer and user
                    await _customerRepository.CreateCustomerAsync(customer);
                    await _customerRepository.SaveChangesAsync();
                }
                else
                {
                    // For Admin/Staff/Moderator, only create User
                    await _userRepository.CreateAsync(user);
                    await _userRepository.SaveChangesAsync();
                }

                // ========== STEP 4: CREATE JWT TOKEN ==========

                var token = _tokenService.CreateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    Email = user.Email,
                    ExpireTime = DateTime.UtcNow.AddMinutes(60)
                };

                return Result<AuthResponse>.Success(
                    response,
                    $"{user.Role} user successfully registered.",
                    201
                );
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(
                    $"An error occurred during registration: {ex.Message}",
                    500
                );
            }
        }

        /// <summary>
        /// Retrieves all admin users from the system.
        /// 
        /// Workflow:
        /// 1. Query User table for users with Admin role
        /// 2. Map to AdminsDto (Id, Email, Name, Surname)
        /// 3. Return list
        /// 
        /// Note: Admin users do not have a customer profile, only a User record exists
        /// </summary>
        public async Task<Result<List<AdminsDto>>> GetAllAdminsAsync()
        {
            try
            {
                // Retrieve all admin users
                var admins = await _userRepository.GetAllAdminsByRoleAsync();

                // Return error if no admin users found
                if (!admins.Any())
                    return Result<List<AdminsDto>>.Failure("No admin users found in the system.");

                // Map to AdminsDto list
                var adminDtos = admins.Select(u => new AdminsDto
                {
                    Id = u.Id,
                    Email = u.Email
                }).ToList();

                return Result<List<AdminsDto>>.Success(adminDtos);
            }
            catch (Exception ex)
            {
                return Result<List<AdminsDto>>.Failure(
                    $"Admin listesi getirilirken hata oluştu: {ex.Message}",
                    500
                );
            }
        }
    }
}