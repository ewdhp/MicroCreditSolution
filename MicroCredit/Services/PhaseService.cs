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
                .GetRequiredService<ApprovalService>(),
                CStatus.Approved => _serviceProvider
                .GetRequiredService<PayService>(),
                _ => throw new ArgumentOutOfRangeException
                (nameof(status), status, null)
            };
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
            try
            {
                var req = request as InitialRequest;
                Guid userId = _userCS.GetUserId();
                if (userId == Guid.Empty)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                var loan = new Loan
                {
                    UserId = userId,
                    Status = CStatus.Pending,
                    Amount = req.Amount,
                    EndDate = DateTime
                    .SpecifyKind(
                        req.EndDate,
                        DateTimeKind.Utc
                    ),
                };

                await _dbContext.Loans.AddAsync(loan);
                await _dbContext.SaveChangesAsync();

                IPhaseRes response = new
                InitialResponse
                { Status = loan.Status };

                _logger.LogInformation("Status Pending");

                return await
                Task.FromResult
                ((true, response));
            }
            catch (Exception ex)
            {
                _logger.LogError($"InitialPhase Error: {ex.Message}");
                return await Task.FromResult((false, (IPhaseRes)null));
            }
        }
    }

    public class ApprovalService : IPhase
    {
        private readonly ILogger<ApprovalService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;

        public ApprovalService(
            ILogger<ApprovalService> logger,
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

                var existingLoan = await _dbContext.Loans
                .FirstOrDefaultAsync(l => l.UserId == userId &&
                l.Status == CStatus.Pending);
                if (existingLoan == null)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                existingLoan.Status = CStatus.Approved;
                _dbContext.Loans.Update(existingLoan);
                await _dbContext.SaveChangesAsync();

                IPhaseRes response = new InitialResponse
                {
                    Status = existingLoan.Status
                };

                _logger.LogInformation("Status Approved");
                return await Task.FromResult
                ((true, response));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult
                ((false, (IPhaseRes)null));
            }
        }
    }

    public class PayService : IPhase
    {
        private readonly ILogger<PayService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;

        public PayService(
            ILogger<PayService> logger,
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
                var req = request as ApprovalRequest;

                Guid userId = _userCS.GetUserId();
                if (userId == Guid.Empty)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                var existingLoan = await _dbContext.Loans
                .FirstOrDefaultAsync(l => l.UserId == userId &&
                l.Status == CStatus.Approved);
                if (existingLoan == null)
                    return await Task.FromResult
                    ((false, (IPhaseRes)null));

                existingLoan.Status = CStatus.Paid;
                _dbContext.Loans.Update(existingLoan);
                await _dbContext.SaveChangesAsync();

                IPhaseRes response = new InitialResponse
                {
                    Status = existingLoan.Status
                };

                _logger.LogInformation("Status updated to Paid");
                return await Task.FromResult
                ((true, response));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult
                ((false, (IPhaseRes)null));
            }
        }
    }
}