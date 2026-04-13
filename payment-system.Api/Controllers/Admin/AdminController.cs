using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.DTOs.Admin;
using payment_system.Application.DTOs.Auth;
using payment_system.Application.Services.Interfaces;
using payment_system.Domain.Enums;

namespace payment_system.Api.Controllers.Admin
{
    /// <summary>
    /// Admin management controller for administrative operations.
    /// Only accessible by Admin role users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Security: Only Admin can access this controller
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Retrieves all admin users from the system.
        /// 
        /// Security: Only Admin role users can access this endpoint.
        /// Authorization: [Authorize(Roles = "Admin")]
        /// 
        /// Example Request:
        /// GET /api/admin
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Successful Response (200):
        /// [
        ///   {
        ///     "id": "550e8400-e29b-41d4-a716-446655440000",
        ///     "email": "admin1@example.com",
        ///     "name": "Administrator",
        ///     "surname": "First"
        ///   },
        ///   {
        ///     "id": "550e8400-e29b-41d4-a716-446655440001",
        ///     "email": "admin2@example.com",
        ///     "name": "Administrator",
        ///     "surname": "Second"
        ///   }
        /// ]
        /// 
        /// Failed Response (401):
        /// {
        ///   "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
        ///   "title": "Unauthorized",
        ///   "status": 401,
        ///   "traceId": "..."
        /// }
        /// </summary>
        /// <returns>List of all admin users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<AdminsDto>>> GetAllAdmins()
        {
            var result = await _authService.GetAllAdminsAsync();
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new admin user account.
        /// 
        /// Security: Only Admin role users can create new admins.
        /// Authorization: [Authorize(Roles = "Admin")]
        /// 
        /// Request Body:
        /// {
        ///   "email": "newadmin@example.com",
        ///   "password": "SecurePass123!@#",
        ///   "confirmPassword": "SecurePass123!@#",
        ///   "role": 1  // 1 = Admin (Customer = 0, Admin = 1)
        /// }
        /// 
        /// Successful Response (201):
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "email": "newadmin@example.com",
        ///   "role": 1
        /// }
        /// </summary>
        /// <param name="request">Admin registration request</param>
        /// <returns>Created admin with JWT token</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<dynamic>> CreateAdmin([FromBody] RegisterRequest request)
        {
            // Admin creation should automatically set role to Admin
            // For security, we override the role from the request
            request.Role = UserRole.Admin;

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetAllAdmins), new { email = request.Email }, result.Data);
        }

        /// <summary>
        /// Admin Dashboard Statistics (To be implemented)
        /// 
        /// Expected features:
        /// - Total customer count
        /// - Total transaction count
        /// - Daily revenue
        /// - Recent login activity
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<dynamic> GetDashboard()
        {
            // TODO: Return statistics data
            return Ok(new
            {
                message = "Admin Dashboard - To be implemented",
                totalCustomers = 0,
                totalTransactions = 0,
                dailyRevenue = 0
            });
        }
    }
}
