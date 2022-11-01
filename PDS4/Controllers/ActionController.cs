using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using PDS3.Commands.IOFile;
using PDS3.Commands.MD5;
using PDS3.Commands.RC5;
using PDS3.Models;
using static PDS3.Commands.RC5.RC5;
using ErrorViewModel = PDS4.Models.ErrorViewModel;

namespace PDS4.Controllers
{
    public class ActionController : Controller
    {
        private readonly ILogger<ActionController> _logger;
        private static string FileExtension = "";

        public ActionController(ILogger<ActionController> logger)
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

        public IActionResult Action(FunctionModel model)
        {
            return View(new FunctionModel());
        }

        [HttpPost]
        public async Task<IActionResult> EncryptRC5(FunctionModel model)
        {
            try
            {
                if (!System.IO.File.Exists(model.FilePath))
                {
                    const string errorMsg = "File not found";
                    _logger.Log(LogLevel.Error, errorMsg);
                    throw new FileNotFoundException(errorMsg);
                }

                FileExtension = Path.GetExtension(model.FilePath);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var encodedFileContent = rc5.Encrypt(byteFile!, hashedKey);

                stopwatch.Stop();
                var time = stopwatch.Elapsed;

                await IOCommand.WriteFile(encodedFileContent, model.Password);

                return Ok(time);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DecryptRC5(FunctionModel model)
        {
            try
            {
                if (!System.IO.File.Exists(model.FilePath))
                {
                    const string errorMsg = "File not found";
                    _logger.Log(LogLevel.Error, errorMsg);
                    throw new FileNotFoundException(errorMsg);
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var encodedFileContent = rc5.Decrypt(byteFile!, hashedKey);

                stopwatch.Stop();
                var time = stopwatch.Elapsed;

                await IOCommand.WriteFile(encodedFileContent, model.FilePath, PaddFilename(model.FilePath, "-dec"));

                return Ok(time);
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static String PaddFilename(string filePath, string padding)
        {
            var fi = new FileInfo(filePath);
            var fn = Path.GetFileNameWithoutExtension(filePath);

            return Path.Combine(fi.DirectoryName, fn + padding + FileExtension);
        }
    }
}