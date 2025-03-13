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
using System.Security.Claims;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/loans")]
    public class LoanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoanController> _logger;
        private readonly IUserContextService _userContextService;

        public LoanController(
            ApplicationDbContext context,
            ILogger<LoanController> logger,
            IUserContextService userContextService)
        {
            _context = context;
            _logger = logger;
            _userContextService = userContextService;
        }

        [HttpPost("create")]
        private async Task<IActionResult> CreateLoan([FromBody] Loan loan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid userId = _userContextService.GetUserId();

            var user = await _context.Users
            .FirstOrDefaultAsync
            (u => u.Id == userId);
            if (user == null)
                return BadRequest
                (new { message = "User not found" });

            var existingLoan = await _context.Loans
            .FirstOrDefaultAsync(l => l.UserId == userId &&
            (l.Status == CStatus.Active || l.Status == CStatus.Due));

            if (existingLoan != null)
                return BadRequest
                (new { message = "User has an active or due loan" });

            loan.UserId = userId;
            loan.StartDate = DateTime.UtcNow;
            loan.EndDate = loan.EndDate.ToUniversalTime();
            loan.Status = CStatus.Initial;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Loan created with ID: " +
            "{LoanId} for User ID: {UserId}", loan.Id, userId);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
        }

        [HttpPut("{id}")]
        private async Task<IActionResult>
        UpdateLoan(Guid id, [FromBody] LoanStatusUpdate loanStatusUpdate)
        {
            if (id != loanStatusUpdate.Id)
                return BadRequest(new { message = "Loan ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid userId;
            try
            {
                userId = _userContextService.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Invalid user ID in token: {Message}", ex.Message);
                return BadRequest(new { message = "Invalid user ID" });
            }

            _logger.LogInformation("Authenticated User ID: {UserId}", userId);

            var existingLoan = await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (existingLoan == null)
            {
                _logger.LogWarning("Loan not found for User ID: {UserId}", userId);
                return NotFound(new { message = "Loan not found" });
            }

            existingLoan.Status = loanStatusUpdate.Status;

            _context.Loans.Update(existingLoan);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Loan status updated with ID: {LoanId} for User ID: {UserId}", loanStatusUpdate.Id, userId);

            return NoContent();
        }

        [HttpGet("user-loans")]
        public async Task<IActionResult> GetUserLoans()
        {
            Guid userId;
            try
            {
                userId = _userContextService.GetUserId();
                var loans = await _context.Loans
                .Where(l => l.UserId == userId)
                .ToListAsync();
                return Ok(loans);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning
                ("Invalid user ID : {Message}", ex.Message);
                return BadRequest
                (new { message = "Invalid user ID" });
            }
        }

        [HttpGet("current-loan")]
        public async Task<IActionResult> CurrentLoan()
        {
            Guid userId;
            try
            {
                userId = _userContextService.GetUserId();
                var existingLoan = await _context.Loans
                .FirstOrDefaultAsync(l => l.UserId == userId &&
                (l.Status == CStatus.Active || l.Status == CStatus.Due));

                if (existingLoan == null)
                    return NotFound
                    (new { message = "No active or due loan found" });
                return Ok(existingLoan);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning
                ("Invalid user ID : {Message}", ex.Message);
                return BadRequest
                (new { message = "Invalid user ID" });
            }
        }

        [HttpGet("{id}")]
        private async Task<IActionResult> GetLoan(Guid id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
                return NotFound(new { message = "Loan not found" });

            return Ok(loan);
        }

        [HttpDelete("{id}")]
        private async Task<IActionResult> DeleteLoan(Guid id)
        {
            try
            {
                Guid userId = _userContextService.GetUserId();
                var loan = await _context.Loans
                .FirstOrDefaultAsync
                (l => l.Id == id && l.UserId == userId);
                if (loan == null)
                    return NotFound
                    (new { message = "Loan not found" });

                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Loan deleted");
                return Ok(new { message = "Loan deleted" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}