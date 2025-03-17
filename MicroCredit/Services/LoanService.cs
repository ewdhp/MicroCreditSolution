using Microsoft.EntityFrameworkCore;
using MicroCredit.Data;
using MicroCredit.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MicroCredit.Services
{
    public class LoanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public LoanService(ApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Loan> GetCurrentLoanAsync()
        {
            var userId = _userContextService.GetUserId();
            return await _context.Loans
            .FirstOrDefaultAsync(l => l.UserId == userId &&
            l.Status != CStatus.Paid);
        }
        public async Task<bool> AreAllLoansPaidAsync()
        {
            var userId = _userContextService.GetUserId();
            return !await _context.Loans
            .AnyAsync(l => l.UserId == userId && l.Status != CStatus.Paid);
        }

        public async Task<(bool Success, Loan Loan)> CreateLoanAsync(decimal amount)
        {
            var userId = _userContextService.GetUserId();
            var existingLoan = await _context.Loans.FirstOrDefaultAsync(l => l.UserId == userId);

            if (existingLoan != null && existingLoan.Status != CStatus.Paid)
            {
                return (false, null);
            }

            if (amount < 100.0m || amount > 300.0m)
            {
                return (false, null);
            }

            var loan = new Loan
            {
                UserId = userId,
                Amount = amount
            };

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return (true, loan);
        }

        private async Task<Loan> GetLoanAsync(Guid id)
        {
            return await _context.Loans.FindAsync(id);
        }

        public async Task<Loan> GetLoanByIdAsync(Guid id)
        {
            return await GetLoanAsync(id);
        }

        public async Task UpdateLoanStatusAsync(int status)
        {
            var userId = _userContextService.GetUserId();
            var existingLoan = await _context.Loans.FirstOrDefaultAsync(l => l.UserId == userId);

            if (existingLoan == null)
            {
                throw new InvalidOperationException("No loan found for this user.");
            }

            existingLoan.Status = (CStatus)status;
            _context.Loans.Update(existingLoan);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Loan>> GetAllLoansAsync()
        {
            var userId = _userContextService.GetUserId();
            return await _context.Loans
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteAllLoansAsync()
        {
            var userId = _userContextService.GetUserId();
            var userLoans = await _context.Loans.Where(l => l.UserId == userId).ToListAsync();

            if (!userLoans.Any())
            {
                throw new InvalidOperationException("No loans found for this user.");
            }

            _context.Loans.RemoveRange(userLoans);
            await _context.SaveChangesAsync();
        }
    }
}