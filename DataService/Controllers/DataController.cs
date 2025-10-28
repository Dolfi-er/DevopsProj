using Microsoft.AspNetCore.Mvc;

namespace DataService.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private static readonly string[] RandomData = new[]
    {
        "Data Point Alpha", "Data Point Beta", "Data Point Gamma", 
        "Data Point Delta", "Data Point Epsilon"
    };

    [HttpGet]
    public IActionResult Get()
    {
        var random = new Random();
        return Ok(new
        {
            id = Guid.NewGuid(),
            value = RandomData[random.Next(RandomData.Length)],
            timestamp = DateTime.Now,
            randomNumber = random.Next(1, 100)
        });
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        return Ok(RandomData.Select((data, index) => new
        {
            id = index + 1,
            name = data,
            created = DateTime.Now.AddHours(-index)
        }));
    }
}