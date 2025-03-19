using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using System;
using MicroCredit.Services;
using MicroCredit.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly IPhaseFactory _phaseFactory;
        private readonly ILogger<PhaseController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PhaseController(
            IPhaseFactory phaseFactory,
            ILogger<PhaseController> logger,
            IServiceProvider serviceProvider)
        {
            _phaseFactory = phaseFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("next-phase")]
        public async Task<IActionResult> NextPhase([FromBody] IPhaseReq request)
        {
            if (request == null)
            {
                _logger.LogError("Request is null.");
                return BadRequest("Request cannot be null.");
            }

            try
            {
                var loanService = _serviceProvider.GetRequiredService<LoanService>();
                var loan = await loanService.GetCurrentLoanAsync();
                CStatus status = CStatus.Initial;
                if (loan != null) status = loan.Status;

                _logger.LogInformation("Current loan status: {Status}", status);

                var phase = _phaseFactory.GetPhase(status);
                if (phase == null)
                {
                    _logger.LogError("ERROR. Phase is null for status: {Status}", status);
                    return NotFound("ERROR. Phase cannot be null.");
                }

                _logger.LogInformation("Sending request to phase: {Request}", request);
                var result = await phase.CompleteAsync(request);

                if (result == null)
                {
                    _logger.LogError("Response is null.");
                    return NotFound("ERROR. Response cannot be null.");
                }

                _logger.LogInformation("Response: {Response}", result);
                return result.Success ? Ok(new { result }) : BadRequest(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the next phase.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}