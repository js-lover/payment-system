using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService; 

        public CustomerController(ICustomerService customerService) 
            => _customerService = customerService;
    }

}