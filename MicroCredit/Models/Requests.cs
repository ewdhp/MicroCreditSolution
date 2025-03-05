using MicroCredit.Interfaces;
using MicroCredit.Services;

namespace MicroCredit.Models
{
    public class LoanRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }

        public IPhase GetPhase()
        {
            return new LoanPhase();
        }
    }

    public class ApprovalRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }

        public IPhase GetPhase()
        {
            return new ApprovalPhase();
        }
    }

    public class DisburseRequest : IPhaseRequest
    {
        public bool Somefield { get; set; }

        public IPhase GetPhase()
        {
            return new DisbursePhase();
        }
    }
}