using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using MicroCredit.Services;
using MicroCredit.Models;



namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController
        (ILogger<PhaseController> logger,
        IServiceProvider serviceProvider) : 
        ControllerBase
    {
        private readonly ILogger<PhaseController> _logger = logger;
        private readonly IServiceProvider _sc = serviceProvider;

        [HttpPost("next")]
        public async Task<IActionResult>
        ProcessNextPhase(PhaseRequest request)
        {
            _logger.LogInformation("Phase request received.");
            try
            {
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
