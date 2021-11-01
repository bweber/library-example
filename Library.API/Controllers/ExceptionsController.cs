using System;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("exceptions")]
    [ApiController]
    public class ExceptionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Throw()
        {
            // Only for demo purposes only to show behavior in cloud provider portal
            throw new Exception("Boom");
        }
    }
}
