using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SenseNet.Web.Api.Mem.Oidc.Controllers
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
        private readonly SenseNetEnvironment _environment;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<SenseNetEnvironment> environment)
        {
            _logger = logger;
            _environment = environment.Value;
        }

        [HttpGet]
        public object Get()
        {
            var rng = new Random();
            return new
            {
                container = _environment.Container?.Name,
                authority = _environment.Authentication?.Authority,
                weather = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                    .ToArray()
            };
        }
    }
}
