using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using MicroCredit.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("UserId claim not found in token.");
                return Unauthorized(new { message = "UserId claim not found in token." });
            }

            _logger.LogInformation("UserId claim found: {UserId}", userId);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            _logger.LogInformation("CreateUser request received for {PhoneNumber}", user.Phone);

            if (_context.Users.Any(u => u.Phone == user.Phone))
            {
                _logger.LogInformation("A user with the same phone number already exists: {PhoneNumber}", user.Phone);
                return Conflict("A user with the same phone number already exists.");
            }

            user.Phone = user.Phone.Trim();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully with phone number: {PhoneNumber}", user.Phone);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("UserId claim not found in token.");
                return Unauthorized(new { message = "UserId claim not found in token." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            user.Name = updatedUser.Name;
            user.Fingerprint = updatedUser.Fingerprint;
            user.Phase = updatedUser.Phase;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User with ID {UserId} updated successfully.", userId);
            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("UserId claim not found in token.");
                return Unauthorized(new { message = "UserId claim not found in token." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return NotFound(new { message = $"User with ID {userId} not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User with ID {UserId} deleted successfully.", userId);
            return NoContent();
        }


    }
}