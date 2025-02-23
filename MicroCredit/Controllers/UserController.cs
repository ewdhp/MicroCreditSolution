using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroCredit.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroCredit.Data;
using System;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task
        <ActionResult<IEnumerable<User>>> GetUsers()
        {
            var userId = User.Claims
            .FirstOrDefault(
                c => c.Type == "UserId")
                ?.Value;

            if (userId == null)
                return Unauthorized();
            if (!Guid.TryParse(userId,
                out var userGuid))
                return Unauthorized();

            var users = await
            _context.Users.Where(
                u => u.Id == userGuid)
                .ToListAsync();

            if (users == null || !users.Any())
                return NotFound("No users found.");

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var userId = User.Claims.FirstOrDefault(
                c => c.Type == "UserId")
                ?.Value;

            if (userId == null || !Guid.TryParse(userId,
            out var userGuid) || id != userGuid)
                return Unauthorized();
            var user = await
            _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(
                    $"User with ID {id} not found.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>>
        CreateUser([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Phone == user.Phone))
                return Conflict(
                    "A user with the same phone number already exists."
                );

            user.Phone = user.Phone.Trim();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser),
            new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>
        UpdateUser(Guid id, [FromBody] User user)
        {
            var userId = User.Claims
            .FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null || !Guid.TryParse(
                userId, out var userGuid) ||
                id != userGuid)
            {
                return Unauthorized();
            }

            if (id != user.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            user.Phone = user.Phone.Trim();
            var existingUser = await
            _context.Users.FindAsync(id);
            if (existingUser != null)
            {
                _context.Entry(existingUser)
                .State = EntityState.Detached;
            }
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound($"User with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var userId = User.Claims
            .FirstOrDefault(c => c.Type == "UserId")
                ?.Value;

            if (userId == null || !Guid.TryParse(
                userId, out var userGuid) ||
                id != userGuid)
                return Unauthorized();
            var user = await
            _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(
                $"User with ID {id} not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}