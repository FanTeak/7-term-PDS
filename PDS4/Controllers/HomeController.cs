using Microsoft.AspNetCore.Mvc;
using PDS4.Models;
using System.Diagnostics;
using PDS4.Commands.IOFile;

namespace PDS4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static string FileExtension = "";

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

        public async Task<IActionResult> Action(FunctionModel model)
        {
            return View(new FunctionModel());
        }

        [HttpPost]
        public async Task<IActionResult> Encrypt(FunctionModel model)
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Decrypt(FunctionModel model)
        {
            return null;
        }

        [HttpPost]
        public IActionResult OpenFile(string filePath, bool dec = false)
        {
            try
            {
                IOCommand.OpenFile(filePath);
                return Ok();
            }
            catch (Exception e)
            {
                const string errorMsg = "File not found";
                _logger.Log(LogLevel.Error, errorMsg);
                return StatusCode(503, errorMsg);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}