using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(AlertRequest alert)
        {
            // Integrate with notification service here if needed
            return Ok($"Alert sent: {alert.Message}");
        }
    }
} 