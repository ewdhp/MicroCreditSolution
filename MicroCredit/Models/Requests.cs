using System;
using MicroCredit.Interfaces;

namespace MicroCredit.Models
{

    public class InitialRequest : IPhaseRequest
    {
        public Loan LoanDetails { get; set; }
    }
    public class CreateRequest : IPhaseRequest
    {
        public Loan LoanDetails { get; set; }
    }
    public class ApprovalRequest : IPhaseRequest
    {
        public Loan LoanDetails { get; set; }
    }
    public class PayRequest : IPhaseRequest
    {
        public Loan LoanDetails { get; set; }
    }

    public class CreateLoanRequest
    {
        public double Amount { get; set; }
    }
    public class UpdateLoanStatusRequest
    {
        public int Status { get; set; }
    }

}