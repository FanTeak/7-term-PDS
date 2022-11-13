using Microsoft.AspNetCore.Mvc;
using PDS5.Models;
using System.Diagnostics;
using PDS5.Commands;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace PDS5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly DSACryptoServiceProvider _dsa = new();
        private static readonly SHA1 _sha1 = SHA1.Create();

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

        public IActionResult Action()
        {
            return View(new FunctionModel());
        }

        public async Task<IActionResult> GenerateSignature(string value, bool isFromFile)
        {
            try
            {
                byte[]? message;

                if (isFromFile)
                {
                    if (!System.IO.File.Exists(value))
                    {
                        const string errorMsg = "File not found";
                        _logger.Log(LogLevel.Error, errorMsg);
                        throw new FileNotFoundException(errorMsg);
                    }

                    message = await IOCommand.ReadFileByte(value);
                }
                else
                {
                    message = Encoding.Default.GetBytes(value);
                }

                if (message == null)
                {
                    throw new ArgumentNullException(value, "Cannot encode signatureValue");
                }

                byte[] hash = _sha1.ComputeHash(message);
                string result = Convert.ToBase64String(_dsa.CreateSignature(hash));
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> SaveSignature(string value, string filePath)
        {
            try
            {
                await IOCommand.WriteFile(filePath, value);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return Error();
            }
        }

        public async Task<IActionResult> Check(string signatureValue, bool isFromFile, string checkValue)
        {
            try
            {
                string? signature;
                byte[]? fileData;

                if (isFromFile)
                {
                    if (!System.IO.File.Exists(signatureValue) || !System.IO.File.Exists(checkValue))
                    {
                        const string errorMsg = "File not found";
                        _logger.Log(LogLevel.Error, errorMsg);
                        throw new FileNotFoundException(errorMsg);
                    }

                    signature = await IOCommand.ReadFileString(signatureValue);
                    fileData = await IOCommand.ReadFileByte(checkValue);
                }
                else
                {
                    signature = signatureValue;
                    fileData = Encoding.Default.GetBytes(checkValue);
                }

                if (signature == null)
                {
                    throw new ArgumentNullException(signatureValue, "Cannot encode signatureValue");
                }

                if (fileData == null)
                {
                    throw new ArgumentNullException(checkValue, "Cannot encode file");
                }

                byte[] hash = _sha1.ComputeHash(fileData);
                bool verified = _dsa.VerifySignature(hash, Convert.FromBase64String(signature));

                return Ok(verified);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}