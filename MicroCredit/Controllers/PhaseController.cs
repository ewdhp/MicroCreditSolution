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

        public PhaseController
        (ILogger<PhaseController> logger,
        IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sc = serviceProvider;
        }

        [HttpPost("next")]
        public async Task<IActionResult>
        ProcessNextPhase([MyBinder]
        IPhaseRequest request)
        {
            _logger.LogInformation("Phase request received.");

            try
            {
                if (request == null)
                {
                    _logger.LogError("Phase request is null.");
                    return BadRequest("Phase request is null.");
                }

                var phaseService = _sc.GetRequiredService<PhaseService>();
                var r = await phaseService.GetPhaseAsync(request);

                return r.Success ? Ok(r) : BadRequest
                (new { Msg = "Phase failed.", Response = r });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in phase request.");
                return StatusCode(500, "Error in phase request.");
            }

        }
    }
}
