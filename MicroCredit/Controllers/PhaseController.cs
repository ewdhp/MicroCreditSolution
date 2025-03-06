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

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhaseController> _logger;
        private readonly Dictionary<int, Func<IPhase>> _phaseFactory = new Dictionary<int, Func<IPhase>>
        {
            { 1, () => new LoanPhaseService() },
            { 2, () => new ApprovalPhaseService() },
            { 3, () => new DisbursementPhaseService() }
        };

        public PhaseController(ApplicationDbContext context, ILogger<PhaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Endpoint that uses the dict to call the correct phase service comparing the 
        // actual user.Phase value, but first checking claims and first or default
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPhaseView()
        {
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

            //if user phase is greater than the dict size, return a 404
            if (user.Phase > _phaseFactory.Count)
            {
                return NotFound(new { message = "Invalid Phase size" });
            }

            var phaseService = _phaseFactory[user.Phase]();
            //if the phaseService is null, return a 404
            if (phaseService == null)
            {
                return NotFound(new { message = "Phase not found" });
            }

            var phaseView = phaseService.GetPhaseView();
            //if the phaseView is null, return a 404
            if (phaseView == null)
            {
                return NotFound(new { message = "Phase view not found" });
            }
            //increment the user phase by one and update the model
            user.Phase++;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(phaseView);
        }
    }
}