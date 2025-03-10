using System;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using Microsoft.AspNetCore.Http;

namespace MicroCredit.Services
{
    public class PhaseService
    {
        private readonly ILogger<PhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public PhaseService(
            ILogger<PhaseService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userContextService = userContextService;
        }


        public async Task<(bool success, string msg)> UpdStatus(
            Guid loanId, CreditStatus currentStatus,
            CreditStatus nextStatus)
        {
            var loan = await GetLoanById(loanId);
            if (loan == null) return
            (false, "Loan not found");
            if (loan.Status != currentStatus)
                return (false, "Status mismatch");
            loan.Status = nextStatus;
            _dbContext.Loans.Update(loan);
            await _dbContext.SaveChangesAsync();
            return (true, "Loan status updated");
        }

        public async Task<Loan> GetLoanById(Guid loanId)
        {
            Guid userId;

            try { userId = _userContextService.GetUserId(); }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

            var loan = await _dbContext.Loans
            .FirstOrDefaultAsync(
                l => l.Id == loanId &&
                l.UserId == userId);

            if (loan == null)
                _logger.LogWarning
                ("Loan not found");
            return loan;
        }

        public CreditStatus GetNextPhase(CreditStatus currentStatus)
        {
            return currentStatus switch
            {
                CreditStatus.Initial => CreditStatus.Pending,
                CreditStatus.Pending => CreditStatus.Approved,
                CreditStatus.Approved => CreditStatus.Accepted,
                CreditStatus.Accepted => CreditStatus.Disbursed,
                CreditStatus.Disbursed => CreditStatus.Active,
                CreditStatus.Active => CreditStatus.Paid,
                CreditStatus.Paid => CreditStatus.Initial,
                CreditStatus.Due => CreditStatus.Canceled,
                CreditStatus.Canceled => CreditStatus.Initial,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(currentStatus), currentStatus, null)
            };
        }

        public async Task<(bool success, string msg)> Reset(Guid loanId)
        {
            var loan = await GetLoanById(loanId);
            if (loan == null) { return (false, "Loan not found"); }
            if (loan.Status != CreditStatus.Paid) return
            (false, "Loan is not in the Paid status.");
            loan.Status = CreditStatus.Initial;
            _dbContext.Loans.Update(loan);
            await _dbContext.SaveChangesAsync();
            return (true, "Loan status reseted");
        }

    }


    public class LoanPhaseService : IPhase
    {
        private readonly ILogger<LoanPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public LoanPhaseService(
            ILogger<LoanPhaseService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public async Task<bool> CompleteAsync(IPhaseRequest request)
        {

            var r = request as LoanRequest;
            var userId = _userContextService.GetUserId();
            if (userId == Guid.Empty) return await
            Task.FromResult(false);

            var loan = new Loan
            {
                UserId = userId,
                Amount = r.Amount,
                EndDate = DateTime
                .SpecifyKind(
                    r.EndDate,
                    DateTimeKind.Utc
                    ),
            };

            _logger.LogInformation("Loan created");

            try
            {
                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError
                ($"Update Loan Error: {ex.Message}");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }

    public class ApprovalPhaseService : IPhase
    {
        private readonly ILogger<LoanPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public ApprovalPhaseService(
            ILogger<LoanPhaseService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public async Task<bool> CompleteAsync(IPhaseRequest request)
        {
            Guid userId;
            try { userId = _userContextService.GetUserId(); }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message); return
            await Task.FromResult(false);
            }

            if (userId == Guid.Empty)
            {
                _logger.LogError("Invalid user ID");
                return await Task.FromResult(false);
            }
            _logger.LogInformation("User ID: {UserId}", userId);


            var approvalRequest = request as ApprovalRequest;


            _logger.LogInformation(
               "SERVICE: ApprovalPhaseService,\n" +
               "METHOD: ProcessPhase\n, " +
               "PARAMETERS: ApprovalRequest\n" +
               "{{ Type:{Type}, Action:{Action} }}\n",
               request.Type,
               request.Action);

            var existingLoan = await _dbContext.Loans
                .FirstOrDefaultAsync(
                l => l.UserId == userId &&
                l.Status == CreditStatus.Pending);

            if (existingLoan == null)
            {
                _logger.LogInformation("Loan Not found");
                return await Task.FromResult(false);
            }

            _logger.LogInformation(
                "Loan found : {Amount}",
                existingLoan.Amount);

            existingLoan.Status = CreditStatus.Approved;
            _dbContext.Loans.Update(existingLoan);
            _logger.LogInformation("Loan updated to Approved");

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Loan saved to database");

            return await Task.FromResult(true);
        }
    }

    public class DisbursementPhaseService : IPhase
    {
        private readonly ILogger<DisbursementPhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DisbursementPhaseService(
            ILogger<DisbursementPhaseService> logger,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CompleteAsync(IPhaseRequest request)
        {
            var userId = _httpContextAccessor
            .HttpContext.User.FindFirst("sub")?.Value;
            _logger.LogInformation(
            "ProcessPhase called with request: {Request}",
            request.Action);
            var disburseRequest = request as DisburseRequest;
            return await Task.FromResult(true);
        }
    }
}