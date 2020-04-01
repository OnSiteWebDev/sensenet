using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SenseNet.Web.Api.Mem.TokenAuth.Controllers
{
    //[Route("api/[controller]")]
    [Route("")]
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly SenseNetEnvironment _snEnvironment;

        public InfoController(ILogger<InfoController> logger,
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
                    system = GetEnvironment()
                };
            }

            return new
            {
                ping = Ping().Result,
            };
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
