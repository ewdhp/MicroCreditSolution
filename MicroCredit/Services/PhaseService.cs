using System;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicroCredit.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MicroCredit.ModelBinders;

namespace MicroCredit.Services
{

    public class PhaseFactory : IPhaseFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PhaseFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPhase GetPhaseService(CStatus status)
        {
            return status switch
            {
                CStatus.Initial => _serviceProvider
                .GetRequiredService<InitialService>(),

                CStatus.Pending => _serviceProvider
                .GetRequiredService<PendingService>(),

                CStatus.Approved => _serviceProvider
                .GetRequiredService<ApprovalService>(),
                // Add other phases as needed
                _ => throw new ArgumentOutOfRangeException
                (nameof(status), status, null)
            };
        }
    }

    public class PhaseService
    {
        private readonly ILogger<PhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;
        private readonly IPhaseFactory _pFactory;

        public PhaseService(
            ILogger<PhaseService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS,
            IPhaseFactory phaseFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
            _pFactory = phaseFactory;
        }

        public async Task<(bool success, string msg)>
        CompletePhase(CStatus current, CStatus next)
        {
            var loan = await GetLoanById();
            if (loan == null) return (false, "Loan not found");
            if (loan.Status != current) return (false, "Status mismatch");

            var phaseService = _pFactory
            .GetPhaseService(current);
            var result = await phaseService
                .CompleteAsync(
                    new PhaseRequest
                    {
                        Type = "Phase",
                        Action = "UpdateStatus"
                    });

            if (!result.Item1) return (false, "Phase service failed");

            loan.Status = next;
            _dbContext.Loans.Update(loan);
            await _dbContext.SaveChangesAsync();
            return (true, "Loan status updated");
        }

        public async Task<Loan> GetLoanById()
        {
            Guid userId;
            try { userId = _userCS.GetUserId(); }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

            var loan = await _dbContext.Loans
            .FirstOrDefaultAsync(l => l.UserId == userId);
            if (loan == null) _logger.LogWarning("Loan not found");

            return loan;
        }

        public CStatus Next(CStatus currentStatus)
        {
            return currentStatus switch
            {
                CStatus.Initial => CStatus.Pending,
                CStatus.Pending => CStatus.Approved,
                CStatus.Approved => CStatus.Accepted,
                CStatus.Accepted => CStatus.Disbursed,
                CStatus.Disbursed => CStatus.Active,
                CStatus.Active => CStatus.Paid,
                CStatus.Paid => CStatus.Initial,
                CStatus.Due => CStatus.Canceled,
                CStatus.Canceled => CStatus.Initial,
                _ => throw new ArgumentOutOfRangeException
                (nameof(currentStatus), currentStatus, null)
            };
        }

        public async Task<(bool success, string msg)> Reset()
        {
            var loan = await GetLoanById();
            if (loan == null) return (false, "Loan not found");
            if (loan.Status != CStatus.Paid)
                return (false, "Loan is not in the Paid status.");

            loan.Status = CStatus.Initial;
            _dbContext.Loans.Update(loan);
            await _dbContext.SaveChangesAsync();

            return (true, "Loan status reset");
        }
    }


    public class InitialService : IPhase
    {
        private readonly ILogger<InitialService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;

        public InitialService(
            ILogger<InitialService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
        }

        public async Task<(bool, IPhaseResponse)>
        CompleteAsync(IPhaseRequest request)
        {

            var r = request as InitialRequest;
            var userId = _userCS.GetUserId();
            if (userId == Guid.Empty)
                return await Task.FromResult
                ((false, (IPhaseResponse)null));

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

            try
            {
                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError
                ($"Update Loan Error: {ex.Message}");
                return await Task.FromResult
                ((false, (IPhaseResponse)null));
            }

            _logger.LogInformation("Loan created");
            return await Task.FromResult
            ((false, (IPhaseResponse)null));
        }
    }

    public class PendingService : IPhase
    {
        private readonly ILogger<PendingService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;

        public PendingService(
            ILogger<PendingService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
        }

        public async Task<(bool, IPhaseResponse)>
        CompleteAsync(IPhaseRequest request)
        {
            Guid userId;

            try { userId = _userCS.GetUserId(); }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult
                ((false, (IPhaseResponse)null));
            }

            if (userId == Guid.Empty)
                return await Task.FromResult
                ((false, (IPhaseResponse)null));
            var approvalRequest = request as PendingRequest;
            var existingLoan = await _dbContext.Loans
                .FirstOrDefaultAsync(
                l => l.UserId == userId &&
                l.Status == CStatus.Pending);
            if (existingLoan == null)
                return await Task.FromResult
                ((false, (IPhaseResponse)null));

            existingLoan.Status = CStatus.Approved;
            _dbContext.Loans.Update(existingLoan);
            _logger.LogInformation("Loan updated to Approved");
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult
            ((false, (IPhaseResponse)null));
        }
    }

    public class ApprovalService : IPhase
    {
        private readonly ILogger<ApprovalService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApprovalService(
            ILogger<ApprovalService> logger,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool, IPhaseResponse)>
        CompleteAsync(IPhaseRequest request)
        {
            var userId = _httpContextAccessor
            .HttpContext.User.FindFirst("sub")?.Value;
            var approvalRequest = request as ApprovalRequest;
            return await Task.FromResult
            ((false, (IPhaseResponse)null));
        }
    }
}