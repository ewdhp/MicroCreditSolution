using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using MicroCredit.Factories;
using MicroCredit.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/phases")]
    public class PhaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhaseController> _logger;

        public PhaseController(ApplicationDbContext context, ILogger<PhaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("next-phase")]
        public async Task<IActionResult> NextPhase([FromBody] IPhaseRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value; // Use "Id" instead of "UserId"
            if (userId == null)
            {
                return Unauthorized(new { message = "Id claim not found in token." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var validator = PhaseValidatorFactory.CreateValidator(request);
            bool isValid = ValidatePhase(request, validator);

            if (isValid)
            {
                user.Phase += 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, currentPhase = user.Phase });
            }

            return Ok(new { success = false, currentPhase = user.Phase });
        }

        private bool ValidatePhase(IPhaseRequest request, IPhase validator)
        {
            // Assuming each phase class has a Validate method
            var method = validator.GetType().GetMethod("Validate");
            if (method != null)
            {
                return (bool)method.Invoke(validator, new object[] { request });
            }
            throw new InvalidOperationException("Validator does not have a Validate method");
        }
    }
}