using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SenseNet.ContentRepository.Storage.Data.MsSqlClient;
using SenseNet.ContentRepository.Storage.Security;
using SenseNet.Search.Querying;

namespace SenseNet.Web.Api.Sql.SearchServer.TokenAuth.Controllers
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
                    database = GetDatabaseName(),
                    searchServer = GetQueryResult(),
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

        private string GetDatabaseName()
        {
            var dp = (MsSqlDataProvider) SenseNet.ContentRepository.Storage.Data.DataStore.DataProvider;
            var db = dp.CreateDataContext(CancellationToken.None);
            using var cn = db.CreateConnection();
            cn.Open();
            using var cmd = db.CreateCommand();
            cmd.Connection = cn;
            cmd.CommandTimeout = 10;
            cmd.CommandText = "SELECT DB_NAME()";
            var result = cmd.ExecuteScalar();
            return result as string;
        }

        private string GetQueryResult()
        {
            using var x = new SystemAccount();
            var ctx = SnQueryContext.CreateDefault();
            var result = SnQuery.Query("Id:<1000 .SORT:Id", ctx);
            return string.Join(", ", result.Hits.Select(x => x.ToString()));
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

                using var handler = new HttpClientHandler();
                if (_environment.IsDevelopment())
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using var http = new HttpClient(handler);
                using var responseMsg = await http.GetAsync(url);
                var content = responseMsg.ToString();

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
    }
}
