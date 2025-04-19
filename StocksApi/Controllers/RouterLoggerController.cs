using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace StocksApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RouterLoggerController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RouterLoggerController(IWebHostEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("api/routerlogger")]
        public async Task<IActionResult> Index()
        {
            string logMessage = null;
            using (StreamReader streamReader = new StreamReader(Request.Body, Encoding.ASCII))
            {
                char[] a = [];
                logMessage =await streamReader.ReadToEndAsync() + "\n";
            }
            string filePath = this._hostingEnvironment.ContentRootPath + "\\RouterLogger.txt";
            System.IO.File.AppendAllText(filePath, logMessage);
            return Ok();
        }
    }
}
