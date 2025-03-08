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

        public async Task<bool> ProcessPhase(IPhaseRequest request)
        {
            Guid userId;

            // Retrieve userId using the UserContextService
            try
            {
                userId = _userContextService.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult(false);
            }

            var loanRequest = request as LoanRequest;

            if (loanRequest == null)
            {
                _logger.LogError("Invalid request type");
                return await Task.FromResult(false);
            }

            _logger.LogInformation(
               "SERVICE: LoanPhaseService,\n" +
               "METHOD: ProcessPhase\n, " +
               "PARAMETERS: LoanRequest\n" +
               "{{ Type:{Type}, Action:{Action} }}\n",
               loanRequest.Type,
               loanRequest.Action);

            if (decimal.TryParse(loanRequest.Amount.ToString(), out decimal amount) && amount <= 0)
            {
                _logger.LogWarning("Invalid loan amount");
                return await Task.FromResult(false);
            }

            if (loanRequest.EndDate <= DateTime.Now || loanRequest.EndDate > DateTime.Now.AddDays(30))
            {
                _logger.LogWarning("Invalid loan end date");
                return await Task.FromResult(false);
            }

            var loan = new Loan
            {
                UserId = userId,
                Amount = loanRequest.Amount,
                EndDate = DateTime.SpecifyKind(loanRequest.EndDate, DateTimeKind.Utc),
            };

            _logger.LogInformation("Loan created : {Amount}", loan.Amount);

            try
            {
                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while saving the loan: {ex.Message}");
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

        public async Task<bool> ProcessPhase(IPhaseRequest request)
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
                _logger.LogInformation(
                    "SERVICE: ApprovalPhaseService Loan Not found");
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

        public async Task<bool> ProcessPhase(IPhaseRequest request)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;
            _logger.LogInformation(
            "ProcessPhase called with request: {Request}",
            request.Action);
            var disburseRequest = request as DisburseRequest;
            return await Task.FromResult(true);
        }
    }
}