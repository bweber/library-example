using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet("version")]
        [ResponseCache(Duration = 2592000, Location = ResponseCacheLocation.Any)]
        public IActionResult Version()
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Library.API.version.txt");
            using var reader = new StreamReader(stream ?? throw new ArgumentNullException());

            return Ok(reader.ReadToEnd().Trim());
        }
    }
}
