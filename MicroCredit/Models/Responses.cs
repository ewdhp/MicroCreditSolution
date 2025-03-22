using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class InitialResponse : IPhaseResponse
    {
        public Loan LoanDetails { get; set; }
    }
    public class CreateResponse : IPhaseResponse
    {
        public Loan LoanDetails { get; set; }
    }
    public class ApprovalResponse : IPhaseResponse
    {
        public Loan LoanDetails { get; set; }
    }
    public class PayResponse : IPhaseResponse
    {
        public Loan LoanDetails { get; set; }
    }

}