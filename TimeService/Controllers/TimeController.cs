using Microsoft.AspNetCore.Mvc;

namespace TimeService.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            timeZone = TimeZoneInfo.Local.DisplayName
        });
    }

    [HttpGet("utc")]
    public IActionResult GetUtc()
    {
        return Ok(new
        {
            utcTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            timeZone = "UTC"
        });
    }
}