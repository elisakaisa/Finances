using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController : ControllerBase
    {
        [HttpGet("ping")]
        [ProducesResponseType<DateTime>(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok(DateTime.Now);
        }
    }
}
