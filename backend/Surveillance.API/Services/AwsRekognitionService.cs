using Surveillance.API.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon;
using System.Net.Http.Headers;

namespace Surveillance.API.Services
{
    public class AwsRekognitionService : IAiAnalysisService
    {
        private readonly AmazonRekognitionClient _rekognitionClient;
        private readonly string _s3Bucket;
        public AwsRekognitionService(IConfiguration config)
        {
            var awsConfig = config.GetSection("CloudProvider:AWS");
            var accessKey = awsConfig["Rekognition:AccessKeyId"];
            var secretKey = awsConfig["Rekognition:SecretAccessKey"];
            var region = awsConfig["Region"];
            _s3Bucket = awsConfig["Rekognition:S3Bucket"];
            _rekognitionClient = new AmazonRekognitionClient(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<AnalyzeResult> AnalyzeImageAsync(AnalyzeImageRequest request)
        {
            byte[] imageBytes;
            if (!string.IsNullOrEmpty(request.ImageBase64))
            {
                imageBytes = Convert.FromBase64String(request.ImageBase64);
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
                imageBytes = await http.GetByteArrayAsync(request.ImageUrl);
            }
            else
            {
                throw new ArgumentException("No image provided.");
            }

            var detectRequest = new DetectLabelsRequest
            {
                Image = new Image { Bytes = new MemoryStream(imageBytes) },
                MaxLabels = 10,
                MinConfidence = 70F
            };
            var response = await _rekognitionClient.DetectLabelsAsync(detectRequest);
            var detections = response.Labels.Select(l => new Detection
            {
                Type = l.Name,
                Confidence = l.Confidence / 100.0,
                Location = "N/A"
            }).ToArray();
            return new AnalyzeResult { Success = true, Detections = detections };
        }

        public async Task<object> AnalyzeVideoAsync(AnalyzeVideoRequest request)
        {
            if (string.IsNullOrEmpty(_s3Bucket) || string.IsNullOrEmpty(request.VideoUrl))
                throw new ArgumentException("VideoUrl must be an S3 key and S3Bucket must be set.");

            var startReq = new StartLabelDetectionRequest
            {
                Video = new Video { S3Object = new S3Object { Bucket = _s3Bucket, Name = request.VideoUrl } },
                MinConfidence = 70F
            };
            var startResp = await _rekognitionClient.StartLabelDetectionAsync(startReq);
            return new { Success = true, JobId = startResp.JobId, Message = "Video analysis started. Poll for results with this JobId." };
        }
    }
} 