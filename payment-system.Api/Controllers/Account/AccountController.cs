using Microsoft.AspNetCore.Mvc;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
            => _accountService = accountService;
    }
}