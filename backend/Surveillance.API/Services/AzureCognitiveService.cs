using Surveillance.API.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.AI.Vision.ImageAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Azure;

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
            var client = new ImageAnalyzerClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
            ImageSource imageSource;
            if (!string.IsNullOrEmpty(request.ImageBase64))
            {
                var bytes = Convert.FromBase64String(request.ImageBase64);
                imageSource = ImageSource.FromBytes(bytes);
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                imageSource = ImageSource.FromUrl(new Uri(request.ImageUrl));
            }
            else
            {
                throw new ArgumentException("No image provided.");
            }
            var analysisOptions = new ImageAnalysisOptions() { Features = ImageAnalysisFeature.Tags };
            var result = await client.AnalyzeAsync(imageSource, analysisOptions);
            var detections = result.Tags.Select(t => new Detection
            {
                Type = t.Name,
                Confidence = t.Confidence,
                Location = "N/A"
            }).ToArray();
            return new AnalyzeResult { Success = true, Detections = detections };
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