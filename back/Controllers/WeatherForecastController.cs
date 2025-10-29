using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IHttpClientFactory httpClientFactory, ILogger<WeatherForecastController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Get current time from TimeService
            var timeClient = _httpClientFactory.CreateClient("TimeService");
            var timeResponse = await timeClient.GetAsync("time");
            
            string currentTime = "Unknown";
            if (timeResponse.IsSuccessStatusCode)
            {
                var timeData = await timeResponse.Content.ReadAsStringAsync();
                var timeJson = JsonDocument.Parse(timeData);
                currentTime = timeJson.RootElement.GetProperty("currentTime").GetString() ?? "Unknown";
            }

            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                GeneratedAt = currentTime
            })
            .ToArray();

            return Ok(new { forecasts, serviceInfo = "Integrated with TimeService" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating weather forecast");
            
            // Fallback without microservice data
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                GeneratedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToArray();

            return Ok(new { forecasts, serviceInfo = "TimeService unavailable - using fallback" });
        }
    }

    [HttpGet("enhanced")]
    public async Task<IActionResult> GetEnhancedForecast()
    {
        try
        {
            var timeClient = _httpClientFactory.CreateClient("TimeService");
            var dataClient = _httpClientFactory.CreateClient("DataService");

            var timeTask = timeClient.GetAsync("time");
            var dataTask = dataClient.GetAsync("data");

            await Task.WhenAll(timeTask, dataTask);

            string currentTime = "Unknown";
            string randomData = "No data";

            if (timeTask.Result.IsSuccessStatusCode)
            {
                var timeData = await timeTask.Result.Content.ReadAsStringAsync();
                var timeJson = JsonDocument.Parse(timeData);
                currentTime = timeJson.RootElement.GetProperty("currentTime").GetString() ?? "Unknown";
            }

            if (dataTask.Result.IsSuccessStatusCode)
            {
                var dataContent = await dataTask.Result.Content.ReadAsStringAsync();
                var dataJson = JsonDocument.Parse(dataContent);
                randomData = dataJson.RootElement.GetProperty("value").GetString() ?? "No data";
            }

            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                GeneratedAt = currentTime,
                DataPoint = randomData
            })
            .ToArray();

            return Ok(new { 
                forecasts, 
                services = new { 
                    timeService = timeTask.Result.IsSuccessStatusCode,
                    dataService = dataTask.Result.IsSuccessStatusCode
                } 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in enhanced forecast");
            return StatusCode(503, new { error = "Microservices unavailable" });
        }
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
    public string? GeneratedAt { get; set; }
    public string? DataPoint { get; set; }
}