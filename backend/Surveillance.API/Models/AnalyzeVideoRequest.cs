namespace Surveillance.API.Models
{
    public record AnalyzeVideoRequest(string VideoUrl, int CameraId, DateTime Timestamp);
} 