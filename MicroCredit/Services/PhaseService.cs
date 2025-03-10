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
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

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
            .FirstOrDefaultAsync
            (l => l.UserId == userId);
            return loan;
        }

        public CStatus GetLoanStatus(CStatus currentStatus)
        {
            return currentStatus switch
            {
                CStatus.Initial => CStatus.Pending,
                CStatus.Pending => CStatus.Approved,
                CStatus.Approved => CStatus.Active,
                CStatus.Active => CStatus.Paid,
                CStatus.Paid => CStatus.Initial,
                _ => throw new ArgumentOutOfRangeException
                (nameof(currentStatus), currentStatus, null)
            };
        }

        public async Task<(bool, IPhaseRes)> Next(IPhaseReq req)
        {
            try
            {
                IPhase phase = null;
                CStatus current = CStatus.Initial;
                var loan = await GetLoanById();
                phase = (loan == null) ?
                _pFactory.GetPhaseService(current) :
                _pFactory.GetPhaseService(loan.Status);
                return await phase.CompleteAsync(req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult
                ((false, (IPhaseRes)null));
            }
        }

        public async Task<(bool, string)> Reset()
        {
            var loan = await GetLoanById();
            if (loan == null) return (false, "From Reset() Loan not found");
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

        public async Task<(bool, IPhaseRes)> CompleteAsync(IPhaseReq request)
        {
            var initialRequest = request as InitialRequest;

            try
            {
                var loan = new Loan
                {
                    UserId = initialRequest.UserId,
                    Amount = initialRequest.Amount,
                    EndDate = DateTime
                    .SpecifyKind(
                        initialRequest.EndDate,
                        DateTimeKind.Utc
                    ),
                };

                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                $"InitialPhase Error: {ex.Message}");
                return await Task.FromResult
                ((false, (IPhaseRes)null));
            }


            //Construct the response:  
            IPhaseRes reponse = null;

            _logger.LogInformation(
            "Loan Status updated to Pending");
            return await Task.FromResult
            ((true, reponse));
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

        public async Task<(bool, IPhaseRes)> CompleteAsync(IPhaseReq request)
        {
            try
            {
                var pendingRequest = request as PendingRequest;

                Guid userId = _userCS.GetUserId();
                if (userId == Guid.Empty)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                var existingLoan = await _dbContext
                .Loans.FirstOrDefaultAsync
                (l => l.UserId == userId &&
                l.Status == CStatus.Pending);
                if (existingLoan == null)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                existingLoan.Status = CStatus.Approved;
                _dbContext.Loans.Update(existingLoan);
                await _dbContext.SaveChangesAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult
                ((false, (IPhaseRes)null));
            }

            //Construct the response:  
            IPhaseRes reponse = null;


            _logger.LogInformation(
            "Status updated to Approved");
            return await Task.FromResult
            ((true, reponse));
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

        public async Task<(bool, IPhaseRes)> CompleteAsync(IPhaseReq request)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;
            var approvalRequest = request as ApprovalRequest;
            return await Task.FromResult((false, (IPhaseRes)null));
        }
    }
}