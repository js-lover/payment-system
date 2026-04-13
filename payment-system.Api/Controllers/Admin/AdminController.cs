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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Get all admin users from the system.
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
        /// Create a new admin user account.
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
            // Set role to Admin for security
            request.Role = UserRole.Admin;

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetAllAdmins), new { email = request.Email }, result.Data);
        }

        /// <summary>
        /// Get admin dashboard statistics. (To be implemented)
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<dynamic> GetDashboard()
        {
            // TODO: Implement dashboard statistics
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
