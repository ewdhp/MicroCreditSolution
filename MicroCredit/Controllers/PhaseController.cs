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


            _logger.LogInformation("Processing phase request: {@Request}", request);

            try
            {
                await Task.Delay(1000);
                return Ok(new { V = "ok" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the phase request.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}