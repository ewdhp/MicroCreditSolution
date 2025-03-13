using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MicroCredit.Interfaces;

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
        public async Task<IActionResult>
            NextPhase(IPhaseReq request)
        {
            _logger.LogInformation
            ("PHASE REQUEST: {Request}", request);
            var p = _phaseFactory.GetPhase(request.Status);
            var (success, res) = await p.CompleteAsync(request);
            _logger.LogInformation("{Success} {Response}", success, res);
            return success ? Ok(new { res }) :
            BadRequest(new { success });
        }
    }
}