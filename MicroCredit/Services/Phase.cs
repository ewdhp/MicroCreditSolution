using System;
using MicroCredit.Data;
using MicroCredit.Models;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MicroCredit.Services
{
    public class PhaseService
    {
        private readonly UDbContext _db;
        private readonly IUCService _user;
        private readonly ILoanService _loan;
        private readonly ILogger<PhaseService> _logger;

        public PhaseService
        (
            IUCService userCS,
            UDbContext dbContext,
            ILoanService loanService,
            ILogger<PhaseService> logger)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _user = userCS ?? throw new ArgumentNullException(nameof(userCS));
            _loan = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IPhaseResponse> GetPhaseAsync(IPhaseRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Phase request is null.");
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                return request.Status switch
                {
                    CStatus.Initial => await Init(request),
                    CStatus.Create => await Create(request),
                    CStatus.Pending => await Approval(request),
                    CStatus.Rejected => await Approval(request),
                    CStatus.Active => await Pay(request),
                    CStatus.Due => await Pay(request),
                    _ => throw new
                    ArgumentOutOfRangeException
                    (nameof(request.Status),
                    request.Status, null)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPhaseAsync.");
                throw;
            }
        }

        public Task<IPhaseResponse> Init(IPhaseRequest request)
        {
            InitialResponse response = new();

            if (request.Status != CStatus.Initial)
            {
                response.Success = false;
                response.Status = CStatus.Unknown;
                return Task.FromResult<IPhaseResponse>(response);
            }
            response.Success = true;
            response.Status = CStatus.Create;
            return Task.FromResult<IPhaseResponse>(response);
        }

        public async Task<IPhaseResponse> Create(IPhaseRequest request)
        {
            CreateResponse response = new();
            CreateRequest r = (CreateRequest)request;

            try
            {
                var currentUser = _user.GetUserId();
                if (request.Status != CStatus.Create ||
                    currentUser == Guid.Empty)
                {
                    response.Success = false;
                    response.Status = CStatus.Unknown;
                    return response;
                }

                var loan = new Loan { Amount = r.Amount };

                await _db.Loans.AddAsync(loan);
                await _db.SaveChangesAsync();

                response.Success = true;
                response.Status = CStatus.Pending;
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error creating the loan.");
                response.Success = false;
                response.Status = CStatus.Unknown;
            }

            return response;
        }

        public async Task<IPhaseResponse> Approval(IPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<IPhaseResponse> Pay(IPhaseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}