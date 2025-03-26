
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MicroCredit.Data;
using MicroCredit.Models;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Services
{
    public interface ILoanService
    {
        Task<(bool Success, Loan Loan)>
        CreateLoanAsync(decimal amount);
        Task<(bool, Loan)> GetCurrentLoanAsync();
        Task<Loan> GetLoanByIdAsync(Guid id);
        Task UpdateLoanStatusAsync(int status);
        Task<List<Loan>> GetAllLoansAsync();
        Task DeleteAllLoansAsync();
        Task<(bool, Loan loan)> AreAllPaidAsync();
    }

    public class LoanService : ILoanService
    {
        private readonly UDbContext _db;
        private readonly IUCService _context;
        private readonly ILogger<LoanService> _logger;

        public LoanService(
            ILogger<LoanService> logger,
            IUCService context,
            UDbContext db)
        {
            _logger = logger ?? throw new
            ArgumentNullException(nameof(logger));
            _context = context ?? throw new
            ArgumentNullException(nameof(context));
            _db = db ?? throw new
            ArgumentNullException(nameof(db));
        }

        public async Task<(bool Success, Loan Loan)>
        CreateLoanAsync(decimal amount)
        {
            // Get the current user's ID
            var userId = _context.GetUserId();
            _logger.LogInformation("\nUSER ID: {id}\n", userId);
            // Check if the user already has any loan that is not in the Paid status
            var existingLoan = await _db.Loans
                .AsNoTracking() // Prevents tracking to avoid concurrency issues
                .FirstOrDefaultAsync
                (l => l.UserId == userId &&
                l.Status != CStatus.Paid);

            if (existingLoan != null)
            {
                // If any loan exists that is not Paid, return failure
                return (false, null);
            }

            // Validate the loan amount
            if (amount < 100.0m || amount > 300.0m)
            {
                return (false, null);
            }

            // Create a new loan for the user
            var loan = new Loan
            {
                UserId = userId, // Use the current user's ID
                Amount = amount,
                Status = CStatus.Initial, // Set the initial status
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            _logger.LogInformation("\nLoan created: {loan}\n", loan);
            try
            {
                // Add the loan to the database
                await _db.Loans.AddAsync(loan);
                await _db.SaveChangesAsync();

                // Return success with the created loan
                return (true, loan);
            }
            catch (DbUpdateException ex)
            {
                return (false, null);
            }
        }
        public async Task<(bool, Loan)> GetCurrentLoanAsync()
        {
            var userId = _context.GetUserId();
            var loan = await _db.Loans.FirstOrDefaultAsync(
                l => l.UserId == userId &&
                (int)l.Status != 7);
            return (loan != null, loan);
        }
        public async Task<Loan> GetLoanByIdAsync(Guid id)
        {
            return await _db.Loans.FindAsync(id);
        }
        public async Task UpdateLoanStatusAsync(int status)
        {
            var userId = _context.GetUserId();
            var existingLoan = await _db.Loans
            .FirstOrDefaultAsync
            (l => l.UserId == userId);
            if (existingLoan == null) throw new
            InvalidOperationException("No loan found.");
            existingLoan.Status = (CStatus)status;
            _db.Loans.Update(existingLoan);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Loan>> GetAllLoansAsync()
        {
            var userId = _context.GetUserId();
            return await _db.Loans.Where
                (l => l.UserId == userId)
                    .ToListAsync();
        }
        public async Task DeleteAllLoansAsync()
        {
            var userId = _context.GetUserId();
            var userLoans = await _db.Loans
             .Where(l => l.UserId == userId)
                .ToListAsync();
            if (!userLoans.Any()) throw new
                InvalidOperationException
                ("No loans found.");
            _db.Loans
            .RemoveRange(userLoans);
            await _db.SaveChangesAsync();
        }
        public async Task<(bool, Loan)> AreAllPaidAsync()
        {
            var userId = _context.GetUserId();
            var allPaid = !await _db.Loans.AnyAsync
            (l => l.UserId == userId &&
            l.Status != CStatus.Paid);
            var loan = await _db.Loans.FirstOrDefaultAsync
            (l => l.UserId == userId && l.Status != CStatus.Paid);
            return (allPaid, loan);
        }
    }
}