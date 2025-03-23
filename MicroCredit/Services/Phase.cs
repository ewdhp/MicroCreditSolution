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

        public PhaseService(IUCService userCS, UDbContext dbContext, ILoanService loanService, ILogger<PhaseService> logger)
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

            if (request.Data == null)
            {
                _logger.LogError("Phase request data is null.");
                throw new ArgumentNullException(nameof(request.Data));
            }

            var status = request.Data.Status;
            _logger.LogInformation("PROCESSING STATUS IN BINDER: {Status}", status);
            try
            {
                return status switch
                {
                    CStatus.Initial => await Init(request),
                    CStatus.Create => await Create(request),
                    CStatus.Pending => await Approval(request),
                    CStatus.Rejected => await Approval(request),
                    CStatus.Active => await Pay(request),
                    CStatus.Due => await Pay(request),
                    _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the phase request.");
                throw;
            }
        }

        public Task<IPhaseResponse> Init(IPhaseRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = new InitialResponse
            {
                Data = request.Data
            };

            return Task.FromResult<IPhaseResponse>(response);
        }

        public Task<IPhaseResponse> Create(IPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IPhaseResponse> Approval(IPhaseRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IPhaseResponse> Pay(IPhaseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}