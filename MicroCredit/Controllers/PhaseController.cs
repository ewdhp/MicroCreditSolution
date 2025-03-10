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
        private readonly PhaseService _phase;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(
            PhaseService phaseService,
            ILogger<PhaseController> logger)
        {
            _phase = phaseService;
            _logger = logger;
        }

        [HttpPost("next-phase")]
        public async Task<IActionResult> NextPhase(IPhaseReq request)
        {
            var (success, response) = await _phase.Next(request);
            return success ? Ok(response) :
            BadRequest(response);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            var (success, msg) = await _phase.Reset();
            return success ? Ok(new { msg }) :
            BadRequest(new { msg });
        }
    }
}