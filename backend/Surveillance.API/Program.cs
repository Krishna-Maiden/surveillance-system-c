using Surveillance.API.Models;
using Surveillance.API.Services;
using Surveillance.API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Surveillance.API.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register repositories
builder.Services.AddSingleton<ICameraRepository, InMemoryCameraRepository>();
builder.Services.AddSingleton<IEventLogRepository, InMemoryEventLogRepository>();

// Register AI service
var config = builder.Configuration;
var cloudProvider = config["CloudProvider:Provider"] ?? "AWS";
if (cloudProvider == "Azure")
{
    builder.Services.AddSingleton<IAiAnalysisService, AzureCognitiveService>();
}
else if (cloudProvider == "Python")
{
    builder.Services.AddSingleton<IAiAnalysisService, PythonEmotionService>();
}
else
{
    builder.Services.AddSingleton<IAiAnalysisService, AwsRekognitionService>();
}

builder.Services.AddSingleton<PythonOcrService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(); // Apply CORS before controllers and SignalR
app.UseAuthorization();
app.MapControllers();
app.MapHub<AiAnalysisHub>("/aihub");

app.Run();
