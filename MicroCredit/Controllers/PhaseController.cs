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

        public PhaseController(
            ApplicationDbContext context,
            ILogger<PhaseController> logger,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;

            _phases = new Dictionary<string, Func<IPhase>>
            {
                { "Loan", () => serviceProvider.GetRequiredService<LoanPhaseService>() },
                { "Approval", () => serviceProvider.GetRequiredService<ApprovalPhaseService>() },
                { "Disbursement", () => serviceProvider.GetRequiredService<DisbursementPhaseService>() }
            };

            _logger.Log(LogLevel.Information,
            new EventId(0, "PhaseServicesInitialized"),
            $"Phase services initialized: {string.Join(", ", _phases.Keys)}");
        }

        [HttpPost("phase")]
        public async Task<IActionResult> Phase([PhaseRequestModelBinder] IPhaseRequest request)
        {
            _logger.Log(LogLevel.Information,
            new EventId(0, "EndpointRequest"),
            (object)null, null,
            (state, exception) => $"ENDPOINT REQUEST IN PhaseController.Phase" +
            " ( IPhaseRequest{{ Type:{request.Type}, Action:{request.Action} }} )");

            if (request == null)
                return BadRequest(new
                { message = "Request is null" });

            var userId = User.Claims
            .FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Id claim not found in token.");
                return Unauthorized(new
                {
                    message = "Id claim not found."
                });
            }

            _logger.Log(LogLevel.Information,
            new EventId(0, "IdClaimFound"),
            (object)null, null, (state, exception)
            => $"Id claim found: {userId}");

            var user = await _context.Users
            .FirstOrDefaultAsync(u =>
            u.Id.ToString() == userId);

            if (user == null)
            {
                _logger.LogWarning(
                "User {Id} not found.", userId);
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            if (user.Phase > 4)
            {
                user.Phase = 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information,
                new EventId(0, "PhaseReset"),
                (object)null, null, (state, exception) =>
                $"User phase reset to 1 for user: {userId}");
            }

            _logger.Log(LogLevel.Information,
            new EventId(0, "UserFound"),
            (object)null, null,
             (state, exception) => $"User found: {user}");

            if (string.IsNullOrEmpty(request.Type))
            {
                _logger.LogWarning("Request Type is null or empty.");
                return BadRequest(new
                {
                    message = "Request Type is null or empty."
                });
            }

            if (string.IsNullOrEmpty(request.Action))
            {
                _logger.LogWarning("Request Action is null or empty.");
                return BadRequest(new { message = "Action is null or empty." });
            }

            _logger.Log(LogLevel.Information, new EventId(0, "RequestDetails"),
            (object)null, null, (state, exception) => $"Request Type: {request.Type}," +
            "Action: {request.Action}");

            if (!_phases.ContainsKey(request.Type))
            {
                _logger.LogWarning("Phase Type not found: {Type}", request.Type);
                return NotFound(new { message = "Phase Type not found." });
            }

            var phaseService = _phases[request.Type]();

            if (phaseService == null)
            {
                _logger.LogWarning("Phase not found: {Type}", request.Type);
                return NotFound(new { message = "Phase Type not found" });
            }

            IPhaseViewResponse phaseViewResponse = null;
            if (request.Action == "validate")
            {
                _logger.Log(LogLevel.Information,
                new EventId(0, "ValidatingPhase"),
                (object)null, null,
                (state, exception) =>
                $"Validating phase: {request.Type}");

                var success = await
                phaseService.ProcessPhase(request);
                if (!success)
                {
                    _logger.LogWarning("Validation failed: {Type}", request.Type);
                    return NotFound(new { message = "Phase service not validated" });
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
                return Ok(new { message = "Phase Validated" });
            else if (request.Action == "view")
                return Ok(phaseViewResponse);
            return NotFound(new
            {
                message = "Action not found"
            });
        }
    }
}