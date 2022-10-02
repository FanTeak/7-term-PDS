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

        public async Task<IActionResult> Action(string toHash = "")
        {
            var result = MD5Core.GetHashString(toHash);

            await IOCommand.WriteFile(result);

            var resultModel = new FunctionModel()
            {
                StartValue = toHash,
                HashedModel = result
            };
            return View(resultModel);
        }

        public async Task<IActionResult> Compare(string filePath, string stringToCompare)
        {
            var result = await IOCommand.ReadFile(filePath);

            if (string.IsNullOrWhiteSpace(result))
            {
                return BadRequest("Incorrect filepath or file data");
            }

            result = MD5Core.GetHashString(result);

            var compareModel = new CompareModel()
            {
                FirstHash = result,
                SecondHash = stringToCompare,
                Equals = result.Equals(stringToCompare)
            };

            return Ok(compareModel);
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

        [HttpPost]
        public async Task<IActionResult> ReadFile(string filePath)
        {
            var result = await IOCommand.ReadFile(filePath);
            
            if (string.IsNullOrWhiteSpace(result))
            {
                return BadRequest("Incorrect filepath or file data");
            }

            result = MD5Core.GetHashString(result);

            return Ok(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}