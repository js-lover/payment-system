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
            // 1. Kullanıcıyı e-posta ile veritabanında ara
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // 2. Kullanıcı yoksa veya silinmişse hata dön
            // Güvenlik Notu: "Email yanlış" demek yerine "Geçersiz giriş" demek daha güvenlidir.
            if (user == null)
            {
                return Result<AuthResponse>.Failure("E-posta veya şifre hatalı.");
            }

            // 3. Şifreyi doğrula (BCrypt kullanarak hash karşılaştırması)
            var isPasswordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Result<AuthResponse>.Failure("E-posta veya şifre hatalı.");
            }

            // 4. Kimlik doğrulandı, şimdi JWT oluştur
            var token = _tokenService.CreateToken(user);

            // 5. Başarılı sonucu paketle ve dön
            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                ExpireTime = DateTime.UtcNow.AddMinutes(60) // Token süresiyle uyumlu olmalı
        
            };

            return Result<AuthResponse>.Success(response);
        }

        /// <summary>
        /// Yeni kullanıcı kaydı (Register)
        /// 
        /// "User-First" Mimarisi:
        /// 1. User (Identity) oluştur → Email, Şifre, Role
        /// 2. Eğer Customer role ise → Customer (Profile) oluştur
        /// 3. Admin/Staff için → Sadece User, Customer olmaz
        /// 4. JWT token döndür
        /// </summary>
        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // ========== ADIM 1: VALIDASYON ==========

                if (request == null)
                    return Result<AuthResponse>.Failure("Geçersiz kayıt isteği.");

                if (string.IsNullOrWhiteSpace(request.Email))
                    return Result<AuthResponse>.Failure("Email adresi boş olamaz.");

                if (string.IsNullOrWhiteSpace(request.Password))
                    return Result<AuthResponse>.Failure("Şifre boş olamaz.");

                if (request.Password != request.ConfirmPassword)
                    return Result<AuthResponse>.Failure("Şifreler uyuşmuyor.");

                if (request.Password.Length < 6)
                    return Result<AuthResponse>.Failure("Şifre en az 6 karakter olmalıdır.");

                // Email zaten kayıtlı mı?
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                    return Result<AuthResponse>.Failure("Bu email adresi zaten kayıtlı.");

                // ========== ADIM 2: USER (IDENTITY) OLUŞTUR ==========

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.Trim(),
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    Role = request.Role,  // ✅ Role'ü request'ten al (Customer, Admin, Staff)
                    CreatedAt = DateTime.UtcNow
                };

                // ========== ADIM 3: CUSTOMER PROFİLİ (Sadece Customer role'ü için) ==========

                if (request.Role == UserRole.Customer)
                {
                    // Customer profili oluştur
                    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
                        return Result<AuthResponse>.Failure("Ad ve soyad boş olamaz (Customer için).");

                    if (string.IsNullOrWhiteSpace(request.NationalId))
                        return Result<AuthResponse>.Failure("TC Kimlik Numarası boş olamaz (Customer için).");

                    // National ID duplicate kontrolü
                    var existingNationalId = await _customerRepository.GetCustomerByNationalIdAsync(request.NationalId);
                    if (existingNationalId != null)
                        return Result<AuthResponse>.Failure("Bu TC Kimlik Numarası zaten kayıtlı.");

                    // Customer oluştur
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

                    // Customer'ı ve User'ı kaydet
                    await _customerRepository.CreateCustomerAsync(customer);
                    await _customerRepository.SaveChangesAsync();
                }
                else
                {
                    // Admin/Staff/Moderator için sadece User oluştur
                    await _userRepository.CreateAsync(user);
                    await _userRepository.SaveChangesAsync();
                }

                // ========== ADIM 4: JWT TOKEN OLUŞTUR ==========

                var token = _tokenService.CreateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    Email = user.Email,
                    ExpireTime = DateTime.UtcNow.AddMinutes(60)
                };

                return Result<AuthResponse>.Success(
                    response,
                    $"{user.Role} kullanıcısı başarıyla kaydedildi.",
                    201
                );
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(
                    $"Kayıt işlemi sırasında hata oluştu: {ex.Message}",
                    500
                );
            }
        }

        /// <summary>
        /// Tüm admin kullanıcılarını getirir
        /// 
        /// İş Akışı:
        /// 1. User tablosundan Role = Admin olan kullanıcıları sorgula
        /// 2. AdminsDto'ya dönüştür (Id, Email, Name, Surname)
        /// 3. Liste döndür
        /// 
        /// Not: Admin'in customer profili yoktur, sadece User record'ı vardır
        /// </summary>
        public async Task<Result<List<AdminsDto>>> GetAllAdminsAsync()
        {
            try
            {
                // Tüm admin kullanıcılarını getir
                var admins = await _userRepository.GetAllAdminsByRoleAsync();

                // Eğer admin bulunamazsa
                if (!admins.Any())
                    return Result<List<AdminsDto>>.Failure("Hiçbir admin kullanıcısı bulunamadı.");

                // AdminsDto listesine dönüştür
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