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
else
{
    builder.Services.AddSingleton<IAiAnalysisService, AwsRekognitionService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AiAnalysisHub>("/aihub");

app.Run();
