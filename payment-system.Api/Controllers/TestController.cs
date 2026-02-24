using Microsoft.AspNetCore.Mvc;

namespace payment_system.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API çalışıyor");
    }
}