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
        private static System.Security.Cryptography.RSACryptoServiceProvider _rsa = new();
        
        private static string FileExtension = "";

        private const int EncipherBlockSizeRSA = 64;
        private const int DecipherBlockSizeRSA = 128;

        public ActionController(ILogger<ActionController> logger)
        {
            _logger = logger;
            IOCommand.FilePath = "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS4\\";
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Action()
        {
            return View(new FunctionModel("Qwerty123", "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS4\\test.txt"));
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

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

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

                RC5 rc5 = new RC5();

                var byteFile = await IOCommand.ReadFile(model.FilePath);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var encoded = Encoding.UTF8.GetBytes(model.Password);
                var hashedKey = MD5.GetMD5HashedKeyForRC5(encoded, KeyLengthInBytesEnum.Bytes_8);

                var encodedFileContent = rc5.Decrypt(byteFile!, hashedKey);

                stopwatch.Stop();
                var time = stopwatch.Elapsed;

                await IOCommand.WriteFile(encodedFileContent, model.FilePath, PadFileName(model.FilePath, "-dec"));

                return Ok(time);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EncryptRSA(FunctionModel model)
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

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var encipheredBytes = new List<byte>
                {
                    Capacity = byteFile!.Length * 2
                };

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await Task.Run(() =>
                {
                    for (int i = 0; i < byteFile.Length; i += EncipherBlockSizeRSA)
                    {
                        var inputBlock = byteFile
                            .Skip(i)
                            .Take(EncipherBlockSizeRSA)
                            .ToArray();

                        encipheredBytes.AddRange(_rsa.Encrypt(
                            inputBlock,
                            fOAEP: false));
                    }
                });

                stopwatch.Stop();
                var time = stopwatch.Elapsed;

                await IOCommand.WriteFile(encipheredBytes!.ToArray(), model.Password);

                return Ok(time);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DecryptRSA(FunctionModel model)
        {
            try
            {
                if (!System.IO.File.Exists(model.FilePath))
                {
                    const string errorMsg = "File not found";
                    _logger.Log(LogLevel.Error, errorMsg);
                    throw new FileNotFoundException(errorMsg);
                }

                var byteFile = await IOCommand.ReadFile(model.FilePath);
                var decipheredBytes = new List<byte>
                {
                    Capacity = byteFile!.Length / 2
                };

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                await Task.Run(() =>
                {
                    for (int i = 0; i < byteFile.Length; i += DecipherBlockSizeRSA)
                    {
                        var inputBlock = byteFile
                            .Skip(i)
                            .Take(DecipherBlockSizeRSA)
                            .ToArray();

                        decipheredBytes.AddRange(_rsa.Decrypt(
                            inputBlock,
                            fOAEP: false));
                    }
                });

                stopwatch.Stop();
                var time = stopwatch.Elapsed;

                await IOCommand.WriteFile(decipheredBytes.ToArray(), model.FilePath, PadFileName(model.FilePath, "-dec"));

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
            filePath = dec ? PadFileName(filePath, "-dec") : filePath;
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

        private static String PadFileName(string filePath, string padding)
        {
            var fi = new FileInfo(filePath);
            var fn = Path.GetFileNameWithoutExtension(filePath);

            return Path.Combine(fi.DirectoryName, fn + padding + FileExtension);
        }
    }
}