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
    public class PhaseController(
        FactoryService phaseFactory,
        ILogger<PhaseController> logger) : ControllerBase
    {
        private readonly FactoryService _factory = phaseFactory;
        private readonly ILogger<PhaseController> _logger = logger;

        [HttpPost("next")]
        public async Task<IActionResult>
        Next([FromBody] IPhaseRequest request)
        {
            if (request == null) return
            BadRequest("Request cannot be null.");
            try
            {
                var p = _factory.Create<PhaseService>();
                IPhaseResponse r = await p.GetPhaseAsync(request);
                return r.Data.Status == CStatus.Unknown ?
                Ok(new { r }) : BadRequest(new { r });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in next phase.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}