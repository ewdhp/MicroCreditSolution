using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using MicroCredit.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public PhaseController(ApplicationDbContext context, ILogger<PhaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("next-phase")]
        public async Task<IActionResult> Phase([ModelBinder(BinderType = typeof(PhaseRequestModelBinder))] IPhaseRequest request)
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

            var phase = request.GetPhase();
            bool isValid = ValidatePhase(request, phase);

            if (isValid)
            {
                user.Phase += 1;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, currentPhase = user.Phase });
            }

            return Ok(new { success = false, currentPhase = user.Phase });
        }

        private bool ValidatePhase(IPhaseRequest request, IPhase phase)
        {
            // Assuming each phase class has a Validate method
            var method = phase.GetType().GetMethod("Validate");
            if (method != null)
            {
                return (bool)method.Invoke(phase, new object[] { request });
            }
            throw new InvalidOperationException("Phase does not have a Validate method");
        }
    }
}