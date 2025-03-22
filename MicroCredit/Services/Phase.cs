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
    public class PhaseService(
        IUCService userCS,
        UDbContext dbContext,
        ILoanService loanService,
        ILogger<PhaseService> logger)
    {
        private readonly UDbContext _db = dbContext;
        private readonly IUCService _user = userCS;
        private readonly ILoanService _loan = loanService;
        private readonly ILogger<PhaseService> _logger = logger;

        public async
        Task<IPhaseResponse>
        GetPhaseAsync(IPhaseRequest request)
        {
            var status = request.Data.Status;

            return status switch
            {
                CStatus.Initial => await Init(request),
                CStatus.Create => await Create(request),
                CStatus.Pending => await Approval(request),
                CStatus.Rejected => await Approval(request),
                CStatus.Active => await Pay(request),
                CStatus.Due => await Pay(request),

                _ => throw new
                ArgumentOutOfRangeException
                (nameof(status), status, null)
            };
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