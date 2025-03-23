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
        private readonly PhaseService _phaseService;
        private readonly ILogger<PhaseController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public PhaseController(PhaseService phaseService, ILogger<PhaseController> logger, IServiceProvider serviceProvider)
        {
            _phaseService = phaseService;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("next")]
        public async Task<IActionResult> Next([FromBody] IPhaseRequest request)
        {
            _logger.LogInformation("PhaseController request received.");

            if (request == null)
            {
                _logger.LogWarning("Next phase request is null.");
                return BadRequest("Request cannot be null.");
            }

            try
            {
                _logger.LogInformation("Resolving PhaseService from service provider.");
                var phaseService = _serviceProvider.GetRequiredService<PhaseService>();
                _logger.LogInformation("Calling GetPhaseAsync.");
                IPhaseResponse response = await phaseService.GetPhaseAsync(request);
                _logger.LogInformation("Next phase request processed. Response: {@Response}", response);

                if (response.Data.Status == CStatus.Unknown)
                {
                    return Ok(new { response });
                }
                else
                {
                    return BadRequest(new { response });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the next phase request.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}