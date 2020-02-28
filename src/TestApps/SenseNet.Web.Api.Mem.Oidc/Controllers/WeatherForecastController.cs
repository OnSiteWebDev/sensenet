using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SenseNet.Web.Api.Mem.Oidc.Controllers
{
    [ApiController]
    [Route("")]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly SenseNetEnvironment _snEnvironment;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IWebHostEnvironment environment, IOptions<SenseNetEnvironment> snEnvironment)
        {
            _logger = logger;
            _environment = environment;
            _snEnvironment = snEnvironment.Value;
        }

        [HttpGet]
        public object Get()
        {
            var rng = new Random();

            if (_environment.IsDevelopment())
            {
                return new
                {
                    ping = Ping().Result,
                    sensenet = _snEnvironment,
                    system = GetEnvironment(),
                    weather = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                        {
                            Date = DateTime.Now.AddDays(index),
                            TemperatureC = rng.Next(-20, 55),
                            Summary = Summaries[rng.Next(Summaries.Length)]
                        })
                        .ToArray()
                };
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }

        private object GetEnvironment()
        {
            var env = Environment.GetEnvironmentVariables();

            var result = new Dictionary<string, object>();
            foreach (string key in env.Keys)
                result.TryAdd(key, env[key]);

            return result;
        }

        public async Task<string> Ping()
        {
            try
            {
                var url = _snEnvironment.Authentication.Authority + "/";

                var handler = new HttpClientHandler();
                if(_environment.IsDevelopment())
                    handler.ServerCertificateCustomValidationCallback = delegate { return true; };
                var http = new HttpClient(handler);
                var rmsg = http.GetAsync(url).GetAwaiter().GetResult();
                var content = rmsg.ToString();

                //WebRequest request = WebRequest.Create(url + "/");
                //request.Method = "GET";
                //WebResponse wr = await request.GetResponseAsync();
                //Stream receiveStream = wr.GetResponseStream();
                //string content;
                //using (var reader = new StreamReader(receiveStream, Encoding.UTF8))
                //    content = reader.ReadToEnd().Trim();

                return content.Length > 200 ? content.Substring(0, 200) : content;
            }
            catch (Exception e)
            {
                var msg = $"{e.GetType().Namespace}: {e.Message}";
                while ((e = e.InnerException) != null)
                    msg += $" INNER: {e.GetType().Namespace}: {e.Message}";
                return msg;
            }
        }
        //public IActionResult Pong()
        //{
        //    _logger.LogInformation($"PONG {DateTime.Now:HH:mm:ss}");
        //    return View((object)$"PONG {_environment.Name} {DateTime.Now:HH:mm:ss}");
        //}

    }
}
