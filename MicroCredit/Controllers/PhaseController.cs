using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using MicroCredit.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MicroCredit.Services;
using Microsoft.Extensions.DependencyInjection;
using MicroCredit.ModelBinders;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhaseController> _logger;
        private readonly Dictionary<string, Func<IPhase>> _phases;

        public PhaseController(ApplicationDbContext context, ILogger<PhaseController> logger, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _phases = new Dictionary<string, Func<IPhase>>
        {
            { "Loan", () => serviceProvider.GetRequiredService<LoanPhaseService>() },
            { "Approval", () => serviceProvider.GetRequiredService<ApprovalPhaseService>() },
            { "Disbursement", () => serviceProvider.GetRequiredService<DisbursementPhaseService>() }
        };

            // Log the phase services to verify they are correctly resolved
            _logger.LogInformation("Phase services initialized: {Phases}", _phases.Keys);
        }

        [HttpPost("phase")]
        public async Task<IActionResult> Phase([PhaseRequestModelBinder] IPhaseRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Request is null.");
                return BadRequest(new { message = "Request is null" });
            }

            _logger.LogInformation("Received request: {Action} {Type}", request.Action, request.Type);

            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Id claim not found in token.");
                return Unauthorized(new { message = "Id claim not found in token." });
            }

            _logger.LogInformation("Id claim found: {Id}", userId);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                _logger.LogInformation("User with Id {Id} not found.", userId);
                return NotFound(new { message = "User not found" });
            }

            _logger.LogInformation("User found: {User}", user);

            if (string.IsNullOrEmpty(request.Type))
            {
                _logger.LogWarning("Request Type is null or empty.");
                return BadRequest(new { message = "Request Type is null or empty" });
            }

            if (string.IsNullOrEmpty(request.Action))
            {
                _logger.LogWarning("Request Action is null or empty.");
                return BadRequest(new { message = "Request Action is null or empty" });
            }

            _logger.LogInformation("Request Type: {Type}, Action: {Action}", request.Type, request.Action);

            if (!_phases.ContainsKey(request.Type))
            {
                _logger.LogWarning("Phase Type not found: {Type}", request.Type);
                return NotFound(new { message = "Phase Type not found" });
            }

            var phaseService = _phases[request.Type]();
            if (phaseService == null)
            {
                _logger.LogWarning("Phase Service not found for Type: {Type}", request.Type);
                return NotFound(new { message = "Phase Type not found" });
            }

            _logger.LogInformation("Phase found: {PhaseService}", request.Type);

            IPhaseViewResponse phaseViewResponse = null;

            if (request.Action == "validate")
            {
                if (!phaseService.ValidatePhase(request))
                {
                    _logger.LogWarning("Phase validation failed for Type: {Type}", request.Type);
                    return NotFound(new { message = "Not validated" });
                }
            }
            else if (request.Action == "view")
            {
                phaseViewResponse = phaseService.GetPhaseView();
                if (phaseViewResponse == null)
                {
                    _logger.LogWarning("Phase view not found for Type: {Type}", request.Type);
                    return NotFound(new { message = "Phase view not found" });
                }
            }
            else
            {
                _logger.LogWarning("Action not found: {Action}", request.Action);
                return NotFound(new { message = "Action not found" });
            }

            user.Phase++;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            if (request.Action == "validate")
            {
                return Ok(new { message = "Phase Validated" });
            }
            else if (request.Action == "view")
            {
                return Ok(phaseViewResponse);
            }

            return NotFound(new { message = "Action not found" });
        }
    }
}