using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Services
{
    public class LoanPhaseService : IPhase
    {
        private readonly ILogger<LoanPhaseService> _logger;

        public LoanPhaseService(ILogger<LoanPhaseService> logger)
        {
            _logger = logger;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            var loanRequest = request as LoanRequest;
            _logger.LogInformation(
               "SERVICE: LoanPhaseService, METHOD: ValidatePhase, " +
               "PARAMETERS: LoanRequest {{ Type:{Type}, Action:{Action} }}",
               loanRequest.Type,
               loanRequest.Action);
            return true;
        }
    }

    public class ApprovalPhaseService : IPhase
    {
        private readonly ILogger<ApprovalPhaseService> _logger;

        public ApprovalPhaseService(ILogger<ApprovalPhaseService> logger)
        {
            _logger = logger;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            _logger.LogInformation("ValidatePhase called with request: {Request}", request.Action);
            var approvalRequest = request as ApprovalRequest;
            return true;
        }
    }

    public class DisbursementPhaseService : IPhase
    {
        private readonly ILogger<DisbursementPhaseService> _logger;

        public DisbursementPhaseService(ILogger<DisbursementPhaseService> logger)
        {
            _logger = logger;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request)
        {
            _logger.LogInformation("ValidatePhase called with request: {Request}", request.Action);
            var disburseRequest = request as DisburseRequest;
            return true;
        }
    }
}