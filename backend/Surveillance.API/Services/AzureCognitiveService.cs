using Surveillance.API.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Azure;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Linq;

namespace Surveillance.API.Services
{
    public class AzureCognitiveService : IAiAnalysisService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _videoIndexerAccountId;
        private readonly string _videoIndexerLocation;
        private readonly string _videoIndexerApiKey;
        public AzureCognitiveService(IConfiguration config)
        {
            var azureConfig = config.GetSection("CloudProvider:Azure:CognitiveServices");
            _endpoint = azureConfig["Endpoint"];
            _apiKey = azureConfig["ApiKey"];
            var viConfig = config.GetSection("CloudProvider:Azure:VideoIndexer");
            _videoIndexerAccountId = viConfig["AccountId"];
            _videoIndexerLocation = viConfig["Location"];
            _videoIndexerApiKey = viConfig["ApiKey"];
        }

        public async Task<AnalyzeResult> AnalyzeImageAsync(AnalyzeImageRequest request)
        {
            // Use Azure Face API for emotion detection
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
            string faceApiUrl = $"{_endpoint.TrimEnd('/')}/face/v1.0/detect?returnFaceAttributes=emotion";
            HttpResponseMessage resp;
            if (!string.IsNullOrEmpty(request.ImageBase64))
            {
                // Send as binary
                var bytes = Convert.FromBase64String(request.ImageBase64);
                using var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                resp = await http.PostAsync(faceApiUrl, content);
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                // Send as JSON with url
                var json = JsonSerializer.Serialize(new { url = request.ImageUrl });
                using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                resp = await http.PostAsync(faceApiUrl, content);
            }
            else
            {
                throw new ArgumentException("No image provided.");
            }
            if (!resp.IsSuccessStatusCode)
            {
                return new AnalyzeResult { Success = false, Detections = Array.Empty<Detection>(), Mood = null };
            }
            var responseString = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var faces = doc.RootElement.EnumerateArray().ToArray();
            string? mood = null;
            if (faces.Length > 0 && faces[0].TryGetProperty("faceAttributes", out var attrs) && attrs.TryGetProperty("emotion", out var emotion))
            {
                // Find the emotion with the highest score
                var emotions = emotion.EnumerateObject().Select(e => new { Name = e.Name, Value = e.Value.GetDouble() });
                var top = emotions.OrderByDescending(e => e.Value).FirstOrDefault();
                if (top != null && top.Value > 0.1) // threshold to avoid noise
                    mood = top.Name;
            }
            // Optionally, you can still use the tags detection as before
            var detections = new Detection[0];
            return new AnalyzeResult { Success = true, Detections = detections, Mood = mood };
        }

        public async Task<object> AnalyzeVideoAsync(AnalyzeVideoRequest request)
        {
            // 1. Get Access Token
            using var http = new HttpClient();
            var tokenUrl = $"https://api.videoindexer.ai/Auth/{_videoIndexerLocation}/Accounts/{_videoIndexerAccountId}/AccessToken?allowEdit=true";
            http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _videoIndexerApiKey);
            var token = await http.GetStringAsync(tokenUrl);
            token = token.Trim('"');

            // 2. Submit video for indexing
            var uploadUrl = $"https://api.videoindexer.ai/{_videoIndexerLocation}/Accounts/{_videoIndexerAccountId}/Videos?accessToken={token}&name=SurveillanceVideo&videoUrl={Uri.EscapeDataString(request.VideoUrl)}&privacy=Private";
            var uploadResp = await http.PostAsync(uploadUrl, null);
            uploadResp.EnsureSuccessStatusCode();
            var uploadContent = await uploadResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(uploadContent);
            var videoId = doc.RootElement.GetProperty("id").GetString();
            return new { Success = true, JobId = videoId, Message = "Video indexing started. Poll for results with this JobId." };
        }
    }
} 