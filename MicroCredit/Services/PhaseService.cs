using MicroCredit.Interfaces;
using MicroCredit.Models;

namespace MicroCredit.Services
{
    public class LoanPhase : IPhase
    {
        public bool Validate(IPhaseRequest request)
        {
            var loanRequest = request as LoanRequest;
            return loanRequest != null && loanRequest.Somefield;
        }

        public bool IsValidTransition(int currentPhase)
        {
            return currentPhase == (int)PhaseEnum.Loan;
        }
    }

    public class ApprovalPhase : IPhase
    {
        public bool Validate(IPhaseRequest request)
        {
            var approvalRequest = request as ApprovalRequest;
            return approvalRequest != null && approvalRequest.Somefield;
        }

        public bool IsValidTransition(int currentPhase)
        {
            return currentPhase == (int)PhaseEnum.Approval;
        }
    }

    public class DisbursePhase : IPhase
    {
        public bool Validate(IPhaseRequest request)
        {
            var disburseRequest = request as DisburseRequest;
            return disburseRequest != null && disburseRequest.Somefield;
        }

        public bool IsValidTransition(int currentPhase)
        {
            return currentPhase == (int)PhaseEnum.Disburse;
        }
    }
}