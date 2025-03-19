using MicroCredit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroCredit.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MicroCredit.Interfaces;

namespace MicroCredit.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/loans")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoanController> _logger;

        public LoanController(ILoanService loanService, ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        [HttpGet("current-loan")]
        public async Task<IActionResult> GetCurrentLoan()
        {
            var loan = await _loanService.GetCurrentLoanAsync();
            if (loan == null)
            {
                _logger.LogDebug("No current loan found for user.");
                return NotFound(new { loan = (Loan)null });
            }
            else
            {
                _logger.LogDebug("Current loan for user is {Loan}", loan);
                return Ok(new { loan });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request)
        {
            try
            {
                _logger.LogInformation("CreateLoan called with amount: {Amount}", request.Amount);
                (bool success, Loan loan) = await _loanService.CreateLoanAsync((decimal)request.Amount);
                if (!success)
                {
                    return BadRequest("A loan already exists for this user.");
                }
                _logger.LogInformation("Loan created successfully with ID: {LoanId}", loan.Id);
                return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the loan.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoan(Guid id)
        {
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    return NotFound();
                }
                return Ok(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the loan.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLoanStatus([FromBody] UpdateLoanStatusRequest request)
        {
            try
            {
                await _loanService.UpdateLoanStatusAsync(request.Status);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the loan status.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLoans()
        {
            try
            {
                var loans = await _loanService.GetAllLoansAsync();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all loans.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllLoans()
        {
            try
            {
                await _loanService.DeleteAllLoansAsync();
                return Ok("All loans have been deleted.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting all loans.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}