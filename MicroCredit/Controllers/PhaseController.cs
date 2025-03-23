using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using System;
using MicroCredit.Services;
using MicroCredit.Models;
using Microsoft.Extensions.DependencyInjection;
using MicroCredit.Models.Binders;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly ILogger<PhaseController> _logger;
        private readonly IServiceProvider _sc;

        public PhaseController(ILogger<PhaseController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sc = serviceProvider;
        }

        [HttpPost("next")]
        public async Task<IActionResult> ProcessNextPhase([MyBinder] IPhaseRequest request)
        {
            _logger.LogInformation("Phase request received.");

            if (request == null)
            {
                _logger.LogWarning("Request cannot be null.");
                return BadRequest("Request cannot be null.");
            }

            // Simulate an asynchronous operation
            await Task.Delay(100);

            return Ok(new { request });
        }
    }
}