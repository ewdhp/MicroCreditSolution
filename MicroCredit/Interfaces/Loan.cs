using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface ILoanService
    {
        Task<(bool Success, Loan Loan)>
        CreateLoanAsync(decimal amount);
        Task<Loan> GetCurrentLoanAsync();
        Task<Loan> GetLoanByIdAsync(Guid id);
        Task UpdateLoanStatusAsync(int status);
        Task<List<Loan>> GetAllLoansAsync();
        Task DeleteAllLoansAsync();
        Task<bool> AreAllPaidAsync();

    }
}