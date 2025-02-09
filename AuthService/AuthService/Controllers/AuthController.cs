using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class AuthController : ControllerBase
    {

        public AuthController()
        {
            
        }

        [HttpGet("/")]
        public IActionResult Get() { 
        
            return Ok("it works");
        }

    }
}
