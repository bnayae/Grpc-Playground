using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebRelay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RelayController : ControllerBase
    {
        private readonly ILogger<RelayController> _logger;

        public RelayController(ILogger<RelayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            await Task.Delay(400);
            return "OK";
        }
    }
}
