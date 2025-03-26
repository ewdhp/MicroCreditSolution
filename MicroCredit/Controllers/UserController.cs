using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using MicroCredit.Models;
using MicroCredit.Services;
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
        private readonly UDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public UserController(
            UDbContext context,
            ILogger<UserController> logger,
            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Id claim not found in token.");
                return Unauthorized(new ErrorResponse { Message = "Id claim not found in token." });
            }

            _logger.LogInformation("Id claim found: {Id}", userId);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                _logger.LogInformation("User with Id {Id} not found.", userId);
                return NotFound(new ErrorResponse { Message = "User not found" });
            }

            return Ok(new UserResponse
            {
                Id = user.Id,
                Phone = user.Phone,
                Name = user.Name,
            });
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Phone == user.Phone || u.Name == user.Name))
            {
                return BadRequest(new ErrorResponse { Message = "User with the same phone or name already exists" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid user data" });
            }
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                return Unauthorized(new ErrorResponse { Message = "Id claim not found in token." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound(new ErrorResponse { Message = "User not found" });
            }

            user.Name = updatedUser.Name;
            user.Phone = updatedUser.Phone;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                return Unauthorized(new ErrorResponse { Message = "Id claim not found in token." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound(new ErrorResponse { Message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User with Id {Id} deleted successfully.", userId);
            // Invalidate the token
            _jwtTokenService.InvalidateToken(userId);

            return NoContent();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                _context.Users.Remove(user);
                _jwtTokenService.InvalidateToken(user.Id.ToString());
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("All users deleted successfully.");
            return NoContent();
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
}