using MicroCredit.Interfaces;
using MicroCredit.Models;
using MicroCredit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(ILogger<PhaseController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("next")]
        public async Task<IActionResult> ProcessNextPhase([FromBody] JObject requestData)
        {
            _logger.LogInformation("Processing next phase request.");
            if (requestData == null || !requestData.ContainsKey("discriminator"))
            {
                return BadRequest("Invalid request format.");
            }

            string discriminator = requestData["discriminator"].ToString();

            try
            {
                switch (discriminator)
                {
                    case "InitialRequest":
                        return await HandleInitialRequest(requestData["data"] as JObject);

                    default:
                        return BadRequest("Unknown request type.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing the request.");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<IActionResult> HandleInitialRequest(JObject data)
        {
            if (data == null || !data.ContainsKey("status"))
            {
                return BadRequest("Missing status field.");
            }

            string status = data["status"].ToString();

            // Simulate processing the request
            await Task.Delay(100); // Simulating async work

            return Ok(new { message = "Request processed", receivedStatus = status });
        }
    }
}
