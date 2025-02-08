using Microsoft.AspNetCore.Mvc;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to the Micro Credit Backend API");
        }
    }
}