using Surveillance.API.Models;
using System.Threading.Tasks;

namespace Surveillance.API.Services
{
    public interface IAiAnalysisService
    {
        Task<AnalyzeResult> AnalyzeImageAsync(AnalyzeImageRequest request); // Supports AWS, Azure, or Python microservice
        Task<object> AnalyzeVideoAsync(AnalyzeVideoRequest request); // object for flexibility (JobId or result)
    }
} 