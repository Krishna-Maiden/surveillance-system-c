using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;
using Surveillance.API.Services;
using Microsoft.AspNetCore.SignalR;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiAnalysisService _aiService;
        public AiController(IAiAnalysisService aiService) => _aiService = aiService;

        [HttpPost("analyze-image")]
        public async Task<IActionResult> AnalyzeImage([FromBody] AnalyzeImageRequest req)
        {
            var result = await _aiService.AnalyzeImageAsync(req);
            return Ok(result);
        }

        [HttpPost("analyze-video")]
        public async Task<IActionResult> AnalyzeVideo([FromBody] AnalyzeVideoRequest req)
        {
            var result = await _aiService.AnalyzeVideoAsync(req);
            return Ok(result);
        }

        [HttpPost("analyze-frame")]
        public async Task<IActionResult> AnalyzeFrame([FromBody] AnalyzeImageRequest req)
        {
            var result = await _aiService.AnalyzeImageAsync(req);
            return Ok(result);
        }
    }

    public class AiAnalysisHub : Hub
    {
        private readonly IAiAnalysisService _aiService;
        public AiAnalysisHub(IAiAnalysisService aiService)
        {
            _aiService = aiService;
        }
        public async Task AnalyzeFrame(string imageBase64, int cameraId)
        {
            var req = new AnalyzeImageRequest(imageBase64, null, cameraId, DateTime.UtcNow);
            var result = await _aiService.AnalyzeImageAsync(req);
            await Clients.Caller.SendAsync("AnalysisResult", result);
        }
    }
} 