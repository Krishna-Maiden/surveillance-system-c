namespace Surveillance.API.Models
{
    public record EventLog(int Id, int CameraId, string EventType, string Description, DateTime Timestamp);
} 