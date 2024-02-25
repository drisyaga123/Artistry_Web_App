using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            return Ok("Hi drisya");
        }
        [HttpPost(Name = "GetMyAddress")]
        public IActionResult GetMyAddress(string addressId)
        {
            return Ok(addressId == "1" ? "Address found" : "Not found");
        }
    }
}
