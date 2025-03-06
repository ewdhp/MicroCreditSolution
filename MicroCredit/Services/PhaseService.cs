using MicroCredit.Interfaces;
using MicroCredit.Models;

namespace MicroCredit.Services
{
    public class LoanPhaseService : IPhase
    {
        public IPhaseViewResponse GetPhaseView()
        {
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            var loanRequest = request as LoanRequest;
            return loanRequest != null;
        }
    }
    public class ApprovalPhaseService : IPhase
    {
        public IPhaseViewResponse GetPhaseView()
        {
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            var approvalRequest = request as ApprovalRequest;
            return approvalRequest != null;
        }
    }
    public class DisbursementPhaseService : IPhase
    {
        public IPhaseViewResponse GetPhaseView()
        {
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            var disburseRequest = request as DisburseRequest;
            return disburseRequest != null;
        }
    }
}