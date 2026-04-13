using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Auth;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Common;

namespace payment_system.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Perform user login and return JWT token.
        /// </summary>
        /// <param name="request">Login request with email and password</param>
        /// <returns>JWT token if successful, error message if failed</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Execute business logic (Service)
            var result = await _authService.LoginAsync(request);

            // Check result
            if (!result.IsSuccess)
            {
                // Return 401 for security
                return Unauthorized(new { message = result.Message });
            }

            // Return 200 OK with token information
            return Ok(result.Data);
        }

        /// <summary>
        /// Register a new user.
        /// 
        /// "User-First" Architecture:
        /// 1. Create user with selected role (Customer, Admin)
        /// 2. Add profile information if Customer
        /// 3. Return JWT token
        /// 
        /// Example Customer Registration:
        /// {
        ///   "email": "customer@example.com",
        ///   "password": "Pass123!",
        ///   "confirmPassword": "Pass123!",
        ///   "role": 0,
        ///   "name": "Ahmet",
        ///   "surname": "Yılmaz",
        ///   "nationalId": "12345678901",
        ///   "phoneNumber": "+905551234567",
        ///   "dateOfBirth": "1990-05-15"
        /// }
        /// 
        /// Example Admin Registration (Profile optional):
        /// {
        ///   "email": "admin@example.com",
        ///   "password": "Pass123!",
        ///   "confirmPassword": "Pass123!",
        ///   "role": 1
        /// }
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>JWT token or error message</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Execute business logic (Service)
            var result = await _authService.RegisterAsync(request);

            // Check result
            if (!result.IsSuccess)
            {
                // Return 400 for security
                return BadRequest(new { message = result.Message });
            }

            // Return 201 Created with token information
            return CreatedAtAction(nameof(Login), new { email = request.Email }, result.Data);
        }


    }
}