using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SenseNet.Web.Mvc.Mem.Admin.Models;

namespace SenseNet.Web.Mvc.Mem.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SenseNetEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IOptions<SenseNetEnvironment> environment)
        {
            _logger = logger;
            _environment = environment.Value;
        }

        public IActionResult Index()
        {
            if(_environment.Container.Name != null)
                return View(_environment);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
