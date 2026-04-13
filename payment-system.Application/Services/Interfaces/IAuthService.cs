using payment_system.Application.Common;
using payment_system.Application.DTOs.Admin;
using payment_system.Application.DTOs.Auth;

namespace payment_system.Application.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user by email and password, validates credentials, and returns a JWT token.
        /// </summary>
        /// <param name="request">Login request containing email and password</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        Task <Result<AuthResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registers a new user account with email, password, and role.
        /// 
        /// Workflow:
        /// 1. Email validation (check if already registered)
        /// 2. Password validation (strength and confirmation match)
        /// 3. Create User (Identity) - Email, password, role
        /// 4. If Customer role, create customer profile
        /// 5. Return JWT token
        /// </summary>
        /// <param name="request">Registration request containing email, password, role, and profile information</param>
        /// <returns>JWT token and user information or error message</returns>
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);


        /// <summary>
        /// Retrieves all admin users from the system.
        /// </summary>
        /// <returns>List of admin users with their details</returns>
        Task<Result<List<AdminsDto>>> GetAllAdminsAsync();
    }
}