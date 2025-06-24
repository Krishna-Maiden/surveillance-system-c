using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        [HttpPost("ingest")]
        public IActionResult Ingest(VideoIngestRequest req)
        {
            // Integrate with video storage/processing here if needed
            return Ok($"Video from camera {req.CameraId} received.");
        }
    }
} 