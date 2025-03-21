using Microsoft.AspNetCore.Mvc;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly IPhaseFactory _phaseFactory;
        private readonly ILogger<PhaseController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PhaseController(IPhaseFactory phaseFactory, ILogger<PhaseController> logger, IServiceProvider serviceProvider)
        {
            _phaseFactory = phaseFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("next")]
        public async Task<IActionResult> NextPhase([FromBody] IPhaseReq request)
        {
            if (request == null)
            {
                _logger.LogDebug("Request is null.");
                return BadRequest(new { message = "Request cannot be null" });
            }

            try
            {
                var loanService = (ILoanService)_serviceProvider.GetService(typeof(ILoanService));
                var currentLoan = await loanService.GetCurrentLoanAsync();
                if (currentLoan == null)
                {
                    _logger.LogDebug("No current loan found for user.");
                    return NotFound(new { message = "Current loan not found" });
                }

                var phase = _phaseFactory.GetPhase(currentLoan.Status);
                if (phase == null)
                {
                    _logger.LogDebug("No phase found for status {Status}", currentLoan.Status);
                    return NotFound(new { message = "Phase not found" });
                }

                var result = await phase.CompleteAsync(request);
                if (result == null)
                {
                    _logger.LogDebug("No result returned for phase {Phase}", phase.GetType().Name);
                    return NotFound(new { message = "Result not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the next phase.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}