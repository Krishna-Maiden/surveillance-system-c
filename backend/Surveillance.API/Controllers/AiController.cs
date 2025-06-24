using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;
using Surveillance.API.Services;

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
    }
} 