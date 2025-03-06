using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class LoanResponse : IPhaseViewResponse
    {
        public string Type => "Loan view";
    }
    public class ApprovalResponse : IPhaseViewResponse
    {
        public string Type => "Approval view";
    }
    public class DisburseResponse : IPhaseViewResponse
    {
        public string Type => "Disburse view";
    }

}