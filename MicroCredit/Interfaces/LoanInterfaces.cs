using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface ILoanService
    {
        Task<Loan> GetCurrentLoanAsync();
        Task<(bool Success, Loan Loan)> CreateLoanAsync(decimal amount);
        Task<Loan> GetLoanByIdAsync(Guid id);
        Task UpdateLoanStatusAsync(int status);
        Task<List<Loan>> GetAllLoansAsync();
        Task DeleteAllLoansAsync();
        Task<bool> AreAllLoansPaidAsync();
        Task<CStatus> ApproveAsync();
    }
}