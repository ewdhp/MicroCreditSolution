using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class InitialResponse : IPhaseRes
    {
        public string Type => "Loan view";
    }
    public class PendingResponse : IPhaseRes
    {
        public string Type => "Approval view";
    }
    public class ApprovalResponse : IPhaseRes
    {
        public string Type => "Disburse view";
    }

}