using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PDS2.Commands;
using PDS2.Models;

namespace PDS2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Action(string toHash)
        {
            var result = await FunctionCommand.Calculate(toHash);
            return View(result);
        }


        [HttpPost]
        public IActionResult OpenFile()
        {
            if (IOCommand.OpenFile())
            {
                return Ok();
            }

            return StatusCode(503, "Can't open file.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}