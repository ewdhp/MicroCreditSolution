using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using System;
using MicroCredit.Services;
using MicroCredit.Models;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly FactoryService _factory;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(FactoryService phaseFactory, ILogger<PhaseController> logger)
        {
            _factory = phaseFactory;
            _logger = logger;
        }

        [HttpPost("next")]
        public async Task<IActionResult> Next([FromBody] IPhaseRequest request)
        {
            _logger.LogInformation("Entering Next method.");
            _logger.LogInformation("Phase controller request received. Request: {@Request}", request);
            if (request == null)
            {
                _logger.LogWarning("Next phase request is null.");
                return BadRequest("Request cannot be null.");
            }

            try
            {
                _logger.LogInformation("Creating phase service.");
                var phaseService = _factory.Create<PhaseService>();
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