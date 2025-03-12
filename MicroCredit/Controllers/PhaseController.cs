using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Services;
using System;
using System.Threading.Tasks;
using MicroCredit.Models;
using MicroCredit.Interfaces;
using MicroCredit.ModelBinders;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly IPhaseFactory _phaseFactory;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(
            IPhaseFactory phaseFactory,
            ILogger<PhaseController> logger)
        {
            _phaseFactory = phaseFactory;
            _logger = logger;
        }

        [HttpPost("next-phase")]
        public async Task<IActionResult> NextPhase(IPhaseReq request)
        {
            _logger.LogInformation("PHASE REQUEST: {Request}", request);
            var phase = _phaseFactory.GetPhaseService(request.Status);
            var (success, response) = await phase.CompleteAsync(request);
            return success ? Ok(new { response }) : BadRequest(new { success });
        }
    }
}