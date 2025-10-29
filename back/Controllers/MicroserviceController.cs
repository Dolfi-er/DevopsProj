using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class MicroserviceController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MicroserviceController> _logger;

    public MicroserviceController(IHttpClientFactory httpClientFactory, ILogger<MicroserviceController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("time")]
    public async Task<IActionResult> GetTime()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("TimeService");
            var response = await client.GetAsync("time");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received time data: {Content}", content);
                return Ok(content);
            }
            
            return StatusCode((int)response.StatusCode, "Time service unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TimeService");
            return StatusCode(503, "Time service unavailable");
        }
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetData()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("DataService");
            var response = await client.GetAsync("data");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received data: {Content}", content);
                return Ok(content);
            }
            
            return StatusCode((int)response.StatusCode, "Data service unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling DataService");
            return StatusCode(503, "Data service unavailable");
        }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var timeClient = _httpClientFactory.CreateClient("TimeService");
            var dataClient = _httpClientFactory.CreateClient("DataService");

            var timeTask = timeClient.GetAsync("time");
            var dataTask = dataClient.GetAsync("data");

            await Task.WhenAll(timeTask, dataTask);

            var timeResponse = timeTask.Result;
            var dataResponse = dataTask.Result;

            var result = new
            {
                timeServiceAvailable = timeResponse.IsSuccessStatusCode,
                dataServiceAvailable = dataResponse.IsSuccessStatusCode,
                timeData = timeResponse.IsSuccessStatusCode ? await timeResponse.Content.ReadAsStringAsync() : null,
                data = dataResponse.IsSuccessStatusCode ? await dataResponse.Content.ReadAsStringAsync() : null
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling microservices");
            return StatusCode(503, new { error = "Microservices unavailable" });
        }
    }
}