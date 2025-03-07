using System;
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

        public bool ValidatePhase(
        IPhaseRequest request, string userId)
        {

            var loanRequest = request as LoanRequest;
            _logger.LogInformation(
               "SERVICE: LoanPhaseService, " +
               "METHOD: ValidatePhase, " +
               "PARAMETERS: LoanRequest " +
               "{{ Type:{Type}, Action:{Action} }}",
               loanRequest.Type,
               loanRequest.Action);

            if (loanRequest != null)
            {
                if (decimal.TryParse(
                    loanRequest.Amount.ToString(),
                    out decimal amount) && amount <= 0)
                {
                    _logger.LogWarning("Invalid loan amount");
                    return false;
                }

                if (loanRequest.EndDate <= DateTime.Now ||
                    loanRequest.EndDate > DateTime.Now.AddDays(30))
                {
                    _logger.LogWarning("Invalid loan end date");
                    return false;
                }
                var loan = new Loan
                {
                    UserId = Guid.Parse(userId),
                    Amount = loanRequest.Amount,
                    EndDate = loanRequest.EndDate,
                };

                _logger.LogInformation(
                "Loan created : {Amount}",
                    loan.Amount
                    );
            }
            else
            {
                _logger.LogError(
                "Invalid request type");
                return false;
            }
            return true;
        }
    }

    public class
    ApprovalPhaseService : IPhase
    {
        private readonly
        ILogger<ApprovalPhaseService> _logger;

        public ApprovalPhaseService(
            ILogger<ApprovalPhaseService> logger)
        {
            _logger = logger;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request, string userId)
        {
            _logger.LogInformation(
                "ValidatePhase called with request: {Request}",
                request.Action
                );
            var approvalRequest = request as ApprovalRequest;
            return true;
        }
    }

    public class DisbursementPhaseService : IPhase
    {
        private readonly
        ILogger<DisbursementPhaseService> _logger;

        public DisbursementPhaseService(
            ILogger<DisbursementPhaseService> logger)
        {
            _logger = logger;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public bool ValidatePhase(IPhaseRequest request, string userId)
        {
            _logger.LogInformation(
                "ValidatePhase called with request: {Request}",
                request.Action);
            var disburseRequest = request as DisburseRequest;
            return true;
        }
    }
}