using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class InitialResponse : IPhaseRes
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public Loan Loan { get; set; }
    }
    public class ApprovalResponse : IPhaseRes
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public Loan Loan { get; set; }
    }
    public class PayResponse : IPhaseRes
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public Loan Loan { get; set; }
    }

}