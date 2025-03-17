using System;
using MicroCredit.Data;
using MicroCredit.Models;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Build.Framework;

namespace MicroCredit.Services
{
    public class PhaseFactory : IPhaseFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InitialService> _logger;

        public PhaseFactory(ILogger<InitialService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public IPhase GetPhase(CStatus status)
        {
            return status switch
            {
                CStatus.Initial => _serviceProvider.GetRequiredService<InitialService>(),
                CStatus.Pending => _serviceProvider.GetRequiredService<ApprovalService>(),
                CStatus.Active => _serviceProvider.GetRequiredService<PayService>(),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }

    public class InitialService : IPhase
    {
        private readonly ILogger<InitialService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;
        private readonly LoanService _loanService;

        public InitialService(
            ILogger<InitialService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS,
            LoanService loanService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
            _loanService = loanService;
        }

        public async Task<IPhaseRes> CompleteAsync(IPhaseReq request)
        {
            try
            {
                _logger.LogInformation("Request received for initial phase.");

                InitialRequest req = request as InitialRequest;
                IPhaseRes response = null;


                var paid = await _loanService.AreAllLoansPaidAsync();
                if (!paid)
                {
                    response = new InitialResponse
                    {
                        Success = false,
                        Msg = "A loan already exists for the user."

                    };
                    _logger.LogError("A loan already exists for the user.");
                    return response;
                }

                var (success, loan) = await _loanService
                .CreateLoanAsync(req.Amount);

                if (!success)
                {
                    _logger.LogError("DB error. Loan not created.");
                    response = new InitialResponse
                    {
                        Success = false,
                        Msg = "DB error. Loan not created."

                    };
                    return response;
                }

                loan.Status = CStatus.Pending;
                _dbContext.Loans.Update(loan);
                await _dbContext.SaveChangesAsync();
                response = new InitialResponse
                {
                    Success = true,
                    Msg = "Loan Created Successfully",
                    Loan = loan

                };

                _logger.LogInformation("Status {Status}", response.Msg);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"InitialPhase Error: {ex.Message}");
                return null;
            }
        }
    }

    public class ApprovalService : IPhase
    {
        private readonly ILogger<ApprovalService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;
        private readonly LoanService _loanService;

        public ApprovalService(
            ILogger<ApprovalService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS,
            LoanService loanService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
            _loanService = loanService;
        }

        public async Task<IPhaseRes> CompleteAsync(IPhaseReq request)
        {
            try
            {
                _logger.LogInformation("Request received for Approval phase.");

                ApprovalRequest req = request as ApprovalRequest;
                IPhaseRes response = null;

                var current = await _loanService.GetCurrentLoanAsync();

                if (current.Status != (CStatus)1)
                {
                    _logger.LogError("Invalid status for approval request.");
                    response = new InitialResponse
                    {
                        Success = false,
                        Msg = "Invalid status for approval request."

                    };
                    return response;
                }

                current.Status = CStatus.Active;
                _dbContext.Loans.Update(current);
                await _dbContext.SaveChangesAsync();
                response = new InitialResponse
                {
                    Success = true,
                    Msg = "Loan Approved Successfully",
                    Loan = current

                };

                _logger.LogInformation("Msg {Status}", response.Msg);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ApprovalPhase Error: {ex.Message}");
                return null;
            }
        }
    }

    public class PayService : IPhase
    {
        private readonly ILogger<PayService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;
        private readonly LoanService _loanService;

        public PayService(
            ILogger<PayService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS,
            LoanService loanService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
            _loanService = loanService;
        }

        public async Task<IPhaseRes> CompleteAsync(IPhaseReq request)
        {
            try
            {
                _logger.LogInformation("Request received for Approval phase.");

                PayRequest req = request as PayRequest;
                IPhaseRes response = null;

                var current = await _loanService.GetCurrentLoanAsync();

                if (current.Status != (CStatus)3)
                {
                    _logger.LogError("Invalid status for paid request.");
                    response = new PayResponse
                    {
                        Success = false,
                        Msg = "Invalid status for paid request."

                    };
                    return response;
                }

                current.Status = CStatus.Paid;
                _dbContext.Loans.Update(current);
                await _dbContext.SaveChangesAsync();
                response = new InitialResponse
                {
                    Success = true,
                    Msg = "Loan Paid Successfully",
                    Loan = current

                };

                _logger.LogInformation("Msg {Status}", response.Msg);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PayPhase Error: {ex.Message}");
                return null;
            }
        }


    }
}
