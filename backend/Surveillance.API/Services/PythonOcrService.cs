using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Surveillance.API.Services
{
    public class PythonOcrService
    {
        private readonly string _pythonServiceUrl;
        public PythonOcrService(IConfiguration config)
        {
            _pythonServiceUrl = config["CloudProvider:Python:OcrEndpoint"] ?? "http://localhost:5005/ocr";
        }

        public async Task<string?> ExtractTextAsync(string imageBase64)
        {
            using var http = new HttpClient();
            var payload = new { imageBase64 };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(_pythonServiceUrl, content);
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }
            var responseString = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            return root.TryGetProperty("text", out var textProp) ? textProp.GetString() : null;
        }
    }
} 