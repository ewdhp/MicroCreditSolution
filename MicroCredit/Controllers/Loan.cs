using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MicroCredit.Services;
using MicroCredit.Models;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/loan")]
    public class LoanController : ControllerBase
    {
        private readonly IServiceProvider _sc;
        private readonly ILogger<LoanController> _logger;

        public LoanController
        (
            ILogger<LoanController> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sc = serviceProvider;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUserLoans()
        {
            _logger.LogInformation
            ("Fetching loans for the current user.");
            try
            {
                var loanService = _sc
                .GetRequiredService<LoanService>();
                var loans = await loanService
                .GetAllLoansAsync();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error fetching user loans.");
                return StatusCode
                (500, "Error fetching user loans.");
            }
        }

        [HttpPost("next")]
        public async Task<IActionResult>
        ProcessNextPhase(PhaseRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Msg = "Invalid request."
                    });
                }

                var ps = _sc.GetRequiredService<PhaseService>();
                var response = await ps.GetPhaseAsync(request);
                return response.Success ? Ok(response) :
                StatusCode(400, new { response });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in phase request.");
                return StatusCode(500, "Error in phase request.");
            }
        }

    }
}
