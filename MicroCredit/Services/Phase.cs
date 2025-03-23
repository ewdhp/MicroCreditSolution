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
                _logger.LogError(ex, "An error occurred in phase request.");
                throw;
            }
        }

        public async Task<IPhaseResponse> Init(IPhaseRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            InitialResponse response = new InitialResponse();
            response.Status = request.Status;


            return response;
        }

        public async Task<IPhaseResponse> Create(IPhaseRequest request)
        {
            throw new NotImplementedException();
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