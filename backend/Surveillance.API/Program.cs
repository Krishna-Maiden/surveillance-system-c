using Surveillance.API.Models;
using Surveillance.API.Services;
using Surveillance.API.Repositories;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
