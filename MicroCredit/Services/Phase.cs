using System;
using MicroCredit.Data;
using MicroCredit.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace MicroCredit.Services
{
    public class PhaseService
    (
        IUCService userCS,
        UDbContext dbContext,
        ILoanService loanService,
        ILogger<PhaseService> logger)
    {
        private readonly UDbContext _db = dbContext ??
        throw new ArgumentNullException(nameof(dbContext));
        private readonly IUCService _user = userCS ??
        throw new ArgumentNullException(nameof(userCS));
        private readonly ILoanService _loan = loanService ??
        throw new ArgumentNullException(nameof(loanService));
        private readonly ILogger<PhaseService> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

        public async
        Task<PhaseResponse>
        GetPhaseAsync(PhaseRequest request)
        {
            try
            {
                if (request == null)
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "Phase request is null."
                    };
                var currentUser = _user.GetUserId();
                if (currentUser == Guid.Empty)
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "User not found."
                    };

                var (sucess, loan) = await
                _loan.GetCurrentLoanAsync();
                CStatus status = CStatus.Unknown;

                if (!sucess && loan == null)
                {
                    status = CStatus.Pre;
                    _logger.LogInformation
                    ($"\nNO LOANS FOUND, STATUS: Pre.\n");
                }
                else
                {
                    status = loan.Status;
                    _logger.LogInformation
                    ($"\nLOAN FOUND, STATUS: {status}.\n");
                }

                return status switch
                {
                    CStatus.Pre => await Pre(),
                    CStatus.Initial => await Init(request),
                    CStatus.Create => await Approval(),
                    CStatus.Pending => await Approval(),
                    CStatus.Rejected => await Approval(),
                    CStatus.Disbursed => await Approval(),
                    CStatus.Active => await Pay(),
                    CStatus.Due => await Pay(),
                    _ => throw new
                    ArgumentOutOfRangeException
                    (nameof(status), status, null)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error GetPhaseAsync.");
                throw;
            }
        }

        private async Task<PhaseResponse> Pre()
        {

            _logger.LogInformation("Pre phase.");
            var newLoan = new Loan
            {
                Amount = 0,
                UserId = _user.GetUserId(),
                Status = CStatus.Initial
            };
            await _db.Loans.AddAsync(newLoan);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Loan created status Pre");

            return new PhaseResponse
            {
                Success = true,
                Msg = "Pre",
                Component = "TakeLoan",
                LoanData = newLoan
            };
        }
        private async Task<PhaseResponse> Init(PhaseRequest request)
        {
            try
            {
                if (request.Init?.Amount == null)
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "Amount is null."
                    };

                var amount = request.Init.Amount;
                var (success, loan) = await
                _loan.GetCurrentLoanAsync();
                loan.Amount = amount;
                loan.Status = CStatus.Pending;
                await _db.SaveChangesAsync();
                return new PhaseResponse
                {
                    Success = true,
                    Msg = "Initial",
                    Component = "LoanInfo",
                    LoanData = loan
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Loan.");
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Error in Loan."
                };
            }
        }
        private async Task<PhaseResponse> Approval()
        {
            var (success, loan) = await _loan
                .GetCurrentLoanAsync();
            if (!success || loan == null)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Error. No loan found."
                };
            }

            if (loan.Status == CStatus.Pending)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Loan Pending.",
                    Component = "LoanInfo",
                    LoanData = loan
                };

            }

            if (loan.Status == CStatus.Rejected)
            {
                loan.Status = CStatus.Pre;
                loan.Amount = 0;
                await _db.SaveChangesAsync();
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Loan rejected.",
                    Component = "LoanInfo",
                    LoanData = loan
                };

            }

            if (loan.Status == CStatus.Disbursed)
            {
                loan.Status = CStatus.Active;
                await _db.SaveChangesAsync();
                return new PhaseResponse
                {
                    Success = true,
                    Msg = "Loan Active.",
                    Component = "LoanInfo",
                    LoanData = loan
                };

            }

            loan.Status = CStatus.Unknown;
            await _db.SaveChangesAsync();
            return new PhaseResponse
            {
                Success = false,
                Msg = "Approval failed",
                Component = "LoanInfo",
                LoanData = loan
            };
        }
        private async Task<PhaseResponse> Pay()
        {
            var (success, loan) = await _loan
                .GetCurrentLoanAsync();
            if (!success || loan == null)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Error. No loan found."
                };
            }

            if (loan.Status == CStatus.Active ||
                loan.Status == CStatus.Due)
            {
                loan.Status = CStatus.Paid;
                await _db.SaveChangesAsync();
                return new PhaseResponse
                {
                    Success = true,
                    Msg = "Loan Paid.",
                    Component = "LoanInfo",
                    LoanData = loan
                };
            }

            return new PhaseResponse
            {
                Success = false,
                Msg = "Loan Not Paid.",
                Component = "LoanInfo",
                LoanData = loan
            };
        }
    }
}