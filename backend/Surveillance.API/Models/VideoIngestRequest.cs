namespace Surveillance.API.Models
{
    public record VideoIngestRequest(int CameraId, string VideoUrl, DateTime Timestamp);
} 