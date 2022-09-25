namespace PDS1.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using Commands;
    using RDS1.Models;

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

        public async Task<IActionResult> Action(InputModel model)
        {
            var a = 1;
            return View();
        }


        [HttpPost]
        public IActionResult OpenFile()
        {
            IOCommand command = new IOCommand();

            if (command.OpenFile())
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