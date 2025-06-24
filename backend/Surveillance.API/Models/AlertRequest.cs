namespace Surveillance.API.Models
{
    public record AlertRequest(int CameraId, string Message, DateTime Timestamp);
} 