using Microsoft.AspNetCore.Mvc;
using MicroCredit.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroCredit.Data;
using System.Text.RegularExpressions;
using System;

namespace MicroCredit.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for duplicate phone number
            if (_context.Users.Any(u => u.PhoneNumber == user.PhoneNumber))
            {
                return Conflict("A user with the same phone number already exists.");
            }

            // Validate phone number format
            if (!Regex.IsMatch(user.PhoneNumber, @"^\d{10}$"))
            {
                return BadRequest("Invalid phone number format. It should be a 10-digit number.");
            }

            // Validate image URLs
            foreach (var imageUrl in user.Images)
            {
                if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return BadRequest("Invalid image URL format.");
                }
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate phone number format
            if (!Regex.IsMatch(user.PhoneNumber, @"^\d{10}$"))
            {
                return BadRequest("Invalid phone number format. It should be a 10-digit number.");
            }

            // Validate image URLs
            foreach (var imageUrl in user.Images)
            {
                if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return BadRequest("Invalid image URL format.");
                }
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
                    throw;  // Re-throw the exception if something unexpected happens
                }
            }

            return Ok(user);  // Return the updated user
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();  // No content to return, just a successful deletion
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
