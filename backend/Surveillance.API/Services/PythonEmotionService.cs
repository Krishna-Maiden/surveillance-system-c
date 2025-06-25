using Surveillance.API.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace Surveillance.API.Services
{
    public class PythonEmotionService : IAiAnalysisService
    {
        private readonly string _pythonServiceUrl;
        public PythonEmotionService(IConfiguration config)
        {
            // Optionally make this configurable
            _pythonServiceUrl = config["CloudProvider:Python:Endpoint"] ?? "http://localhost:5005/analyze";
        }

        public async Task<AnalyzeResult> AnalyzeImageAsync(AnalyzeImageRequest request)
        {
            using var http = new HttpClient();
            var payload = new { imageBase64 = request.ImageBase64 };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(_pythonServiceUrl, content);
            if (!resp.IsSuccessStatusCode)
            {
                return new AnalyzeResult { Success = false, Detections = Array.Empty<Detection>(), Mood = null };
            }
            var responseString = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            var mood = root.TryGetProperty("mood", out var moodProp) ? moodProp.GetString() : null;
            return new AnalyzeResult { Success = true, Detections = Array.Empty<Detection>(), Mood = mood };
        }

        public Task<object> AnalyzeVideoAsync(AnalyzeVideoRequest request)
        {
            // Not implemented for Python microservice
            return Task.FromResult<object>(new { Success = false, Message = "Video analysis not supported in PythonEmotionService." });
        }
    }
} 