using System;
using MicroCredit.Data;
using MicroCredit.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace MicroCredit.Services
{
    public class PhaseService
    {
        private readonly UDbContext _db;
        private readonly IUCService _u;
        private readonly ILoanService _loan;
        private readonly ILogger<PhaseService> _logger;

        public PhaseService(
            IUCService userCS,
            UDbContext dbContext,
            ILoanService loanService,
            ILogger<PhaseService> logger)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _u = userCS ?? throw new ArgumentNullException(nameof(userCS));
            _loan = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PhaseResponse> GetPhaseAsync(PhaseRequest request)
        {
            try
            {
                if (request == null ||
                    _u.GetUserId() == Guid.Empty)
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "REQUEST OR USER NULL"
                    };

                var (sucess, loan) = await
                _loan.GetCurrentLoanAsync();
                if (!sucess) return await Pre(request);
                request.LoanData = loan;
                return request.LoanData.Status switch
                {
                    CStatus.Initial => await Init(request),
                    CStatus.Pending => await Approval(request),
                    CStatus.Rejected => await Approval(request),
                    CStatus.Disbursed => await Approval(request),
                    CStatus.Active => await Pay(request),
                    CStatus.Due => await Pay(request),
                    _ => throw new
                    ArgumentOutOfRangeException
                    (nameof(request.LoanData.Status),
                    request.LoanData.Status, null)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error GetPhaseAsync.");
                throw;
            }
        }
        private async Task<PhaseResponse> Pre(PhaseRequest request)
        {
            _logger.LogInformation("\nPre -> Initial\n");

            try
            {
                var newLoan = new Loan
                {
                    UserId = _u.GetUserId(),
                    Status = CStatus.Initial
                };

                await _db.Loans.AddAsync(newLoan);
                await _db.SaveChangesAsync();

                return new PhaseResponse
                {
                    Success = true,
                    Msg = "Pre",
                    Component = "TakeLoan",
                    LoanData = newLoan
                };
            }
            catch (Exception ex)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "PRE " + ex.Message,
                    Component = "LoanInfo",
                    LoanData = null
                };
            }
        }
        private async Task<PhaseResponse> Init(PhaseRequest request)
        {
            _logger.LogInformation("\nInitial -> Pending\n");

            try
            {
                var loan = request.LoanData;
                if (request.Amount < 100 || request.Amount > 300)
                {
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "Amount error."
                    };
                }
                loan.Status = CStatus.Pending;
                loan.Amount = request.Amount;
                loan.StartDate = DateTime.UtcNow;
                loan.EndDate = loan.StartDate.AddDays(7);
                await _db.SaveChangesAsync();
                return new PhaseResponse
                {
                    Success = true,
                    Msg = "Initial -> Pending",
                    Component = "LoanInfo",
                    LoanData = loan
                };
            }
            catch (Exception ex)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "Init " + ex.Message,
                    Component = "LoanInfo",
                    LoanData = request.LoanData,

                };
            }
        }
        private async Task<PhaseResponse> Approval(PhaseRequest request)
        {
            _logger.LogInformation("\nApproval ->Active\n");

            try
            {
                var loan = request.LoanData;
                if (loan.Status == CStatus.Pending)
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
            }
            catch (Exception ex)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "APPROVAL " + ex.Message,
                    Component = "LoanInfo",
                    LoanData = request.LoanData,

                };
            }

            return new PhaseResponse
            {
                Success = false,
                Msg = "Pay CStatus error.",
                Component = "LoanInfo",
                LoanData = request.LoanData
            };
        }

        private async Task<PhaseResponse> Pay(PhaseRequest request)
        {
            _logger.LogInformation("\nPay ->Paid\n");

            try
            {
                var loan = request.LoanData;
                if (loan.Status == CStatus.Active)
                {
                    loan.Status = CStatus.Paid;
                    await _db.SaveChangesAsync();
                    return new PhaseResponse
                    {
                        Success = true,
                        Msg = "Loan Paid.",
                        Component = "TakeLoan",
                        LoanData = loan
                    };
                }
            }
            catch (Exception ex)
            {
                return new PhaseResponse
                {
                    Success = false,
                    Msg = "PAY " + ex.Message,
                    Component = "LoanInfo",
                    LoanData = request.LoanData,

                };
            }

            return new PhaseResponse
            {
                Success = false,
                Msg = "Pay CStatus error.",
                Component = "LoanInfo",
                LoanData = request.LoanData
            };
        }
    }
}