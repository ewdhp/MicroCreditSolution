using Microsoft.EntityFrameworkCore;
using MicroCredit.Models;
using MicroCredit.Data;
using System.Collections.Generic;
using System.Linq;

namespace MicroCredit.Services  // Ensure this is aligned with the project name
{
    public class LoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ApplyForCredit(Loan model)
        {
            _context.Loans.Add(model);
            _context.SaveChanges();
        }

        public Loan GetCreditDetails(int id)
        {
            return _context.Loans.Find(id)!;
        }

        public List<Loan> GetAllCredits()
        {
            return _context.Loans.ToList();
        }
    }
}
