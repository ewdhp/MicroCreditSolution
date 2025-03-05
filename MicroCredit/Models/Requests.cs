using MicroCredit.Interfaces;
using MicroCredit.Services;

namespace MicroCredit.Models
{
    public class LoanRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }
        public string RequestType => "LoanRequest";

        public IPhase GetPhase()
        {
            return new LoanPhase();
        }
    }

    public class ApprovalRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }
        public string RequestType => "ApprovalRequest";

        public IPhase GetPhase()
        {
            return new ApprovalPhase();
        }
    }

    public class DisburseRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }
        public string RequestType => "DisburseRequest";

        public IPhase GetPhase()
        {
            return new DisbursePhase();
        }
    }

    public class PhaseResetRequest
    {
        public int Phase { get; set; }
    }

    public enum PhaseEnum
    {
        Loan = 1,
        Approval = 2,
        Disburse = 3
    }
}