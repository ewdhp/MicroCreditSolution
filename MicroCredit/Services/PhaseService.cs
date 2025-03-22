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
    public class PhaseFactory : IPhaseFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PhaseFactory> _logger;

        public PhaseFactory
            (ILogger<PhaseFactory> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public async Task<IPhaseResponse>
            GetPhaseAsync(IPhaseRequest request)
        {
            var phaseService = _serviceProvider
            .GetRequiredService<PhaseService>();
            CStatus status = request
            .LoanDetails.Status;

            return status switch
            {
                CStatus.Initial => await phaseService.Init(request),
                CStatus.Create => await phaseService.Create(request),
                CStatus.Pending => await phaseService.Approval(request),
                CStatus.Rejected => await phaseService.Approval(request),
                CStatus.Active => await phaseService.Pay(request),
                CStatus.Due => await phaseService.Pay(request),
                _ => throw new ArgumentOutOfRangeException
                (nameof(status), status, null)
            };
        }
    }

    public class PhaseService : IPhaseService
    {
        private readonly ILogger<PhaseService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserContextService _userCS;
        private readonly ILoanService _loanService;

        public PhaseService(
            ILogger<PhaseService> logger,
            ApplicationDbContext dbContext,
            IUserContextService userCS,
            ILoanService loanService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userCS = userCS;
            _loanService = loanService;
        }


        public Task<IPhaseResponse> Init(IPhaseRequest request)
        {
            throw new NotImplementedException();
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