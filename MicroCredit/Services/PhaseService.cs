using System;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;

namespace MicroCredit.Services
{
    public class LoanPhaseService : IPhase
    {
        private readonly ILogger<LoanPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public LoanPhaseService(
            ILogger<LoanPhaseService> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public async Task<bool> ValidatePhase(IPhaseRequest request, string userId)
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
                    await Task.FromResult(false);
                }

                if (loanRequest.EndDate <= DateTime.Now ||
                    loanRequest.EndDate > DateTime.Now.AddDays(30))
                {
                    _logger.LogWarning("Invalid loan end date");
                    await Task.FromResult(false);
                }
                var loan = new Loan
                {
                    UserId = Guid.Parse(userId),
                    Amount = loanRequest.Amount,
                    EndDate = DateTime.SpecifyKind(
                    loanRequest.EndDate, DateTimeKind.Utc),
                };

                _logger.LogInformation(
                "Loan created : {Amount}",
                    loan.Amount
                    );

                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogError(
                    "Invalid request type");
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
    }

    public class ApprovalPhaseService : IPhase
    {
        private readonly ILogger<ApprovalPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ApprovalPhaseService(
            ILogger<ApprovalPhaseService> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public async Task<bool> ValidatePhase(IPhaseRequest request, string userId)
        {
            var approvalRequest = request as ApprovalRequest;
            var existingLoan = await _dbContext.Loans
            .FirstOrDefaultAsync(
            l => l.UserId == Guid.Parse(userId) &&
            (l.Status == CreditStatus.Active ||
             l.Status == CreditStatus.Due));

            if (existingLoan != null) await Task.FromResult(false);

            existingLoan.Status = CreditStatus.Approved;
            _dbContext.Loans.Update(existingLoan);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Loan approved");
            return await Task.FromResult(true);
        }
    }

    public class DisbursementPhaseService : IPhase
    {
        private readonly ILogger<DisbursementPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public DisbursementPhaseService(
            ILogger<DisbursementPhaseService> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IPhaseViewResponse GetPhaseView()
        {
            _logger.LogInformation("GetPhaseView called");
            throw new System.NotImplementedException();
        }

        public async Task<bool> ValidatePhase(IPhaseRequest request, string userId)
        {
            _logger.LogInformation(
            "ValidatePhase called with request: {Request}",
            request.Action);
            var disburseRequest = request as DisburseRequest;
            return await Task.FromResult(true);
        }
    }
}