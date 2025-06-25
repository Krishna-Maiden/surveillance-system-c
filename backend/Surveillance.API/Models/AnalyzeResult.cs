namespace Surveillance.API.Models
{
    public record AnalyzeResult
    {
        public bool Success { get; set; }
        public Detection[] Detections { get; set; } = Array.Empty<Detection>();
        public string? Mood { get; set; }
    }
    public record Detection
    {
        public string Type { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Location { get; set; } = string.Empty;
    }
} 