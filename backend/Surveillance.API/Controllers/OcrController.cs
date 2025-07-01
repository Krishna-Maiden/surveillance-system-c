using Microsoft.AspNetCore.Mvc;
using Surveillance.API.Models;
using Surveillance.API.Services;
using System.Threading.Tasks;

namespace Surveillance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OcrController : ControllerBase
    {
        private readonly PythonOcrService _ocrService;
        public OcrController(PythonOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] AnalyzeImageRequest req)
        {
            var text = await _ocrService.ExtractTextAsync(req.ImageBase64);
            if (text == null)
                return StatusCode(500, new { success = false, error = "OCR failed" });
            return Ok(new { success = true, text });
        }
    }
} 