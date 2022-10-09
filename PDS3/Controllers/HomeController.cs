using Microsoft.AspNetCore.Mvc;
using PDS3.Models;
using System.Diagnostics;
using System.Text;
using PDS3.Commands.IOFile;
using PDS3.Commands.MD5;
using PDS3.Commands.RC5;
using static PDS3.Commands.RC5.RC5;
using System.Reflection;

namespace PDS3.Controllers
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

        public async Task<IActionResult> Action(FunctionModel model)
        {
            return View(new FunctionModel());
        }

        [HttpPost]
        public async Task<IActionResult> Encrypt(FunctionModel model)
        {
            try
            {
                if (!System.IO.File.Exists(model.FilePath))
                {
                    const string errorMsg = "File not found";
                    _logger.Log(LogLevel.Error, errorMsg);
                    throw new FileNotFoundException(errorMsg);
                }

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var encodedFileContent = rc5.EncipherCBCPAD(byteFile!, hashedKey);

                await IOCommand.WriteFile(encodedFileContent, model.Password);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Decrypt(FunctionModel model)
        {
            try
            {
                if (!System.IO.File.Exists(model.FilePath))
                {
                    const string errorMsg = "File not found";
                    _logger.Log(LogLevel.Error, errorMsg);
                    throw new FileNotFoundException(errorMsg);
                }

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var encodedFileContent = rc5.DecipherCBCPAD(byteFile!, hashedKey);


                await IOCommand.WriteFile(encodedFileContent, model.Password, PaddFilename(model.FilePath, "-dec"));

                return Ok();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public IActionResult OpenFile(string filePath, bool dec = false)
        {
            filePath = dec ? PaddFilename(filePath, "-dec") : filePath;
            if (IOCommand.OpenFile(filePath))
            {
                return Ok();
            }

            const string errorMsg = "File not found";
            _logger.Log(LogLevel.Error, errorMsg);
            return StatusCode(503, errorMsg);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private static String PaddFilename(string filePath, string padding)
        {
            var fi = new FileInfo(filePath);
            var fn = Path.GetFileNameWithoutExtension(filePath);

            return Path.Combine(fi.DirectoryName, fn + padding + fi.Extension);
        }
    }
}