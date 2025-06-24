namespace Surveillance.API.Models
{
    public record AnalyzeImageRequest(string? ImageBase64, string? ImageUrl, int CameraId, DateTime Timestamp);
} 