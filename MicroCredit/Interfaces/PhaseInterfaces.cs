using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface IPhaseService
    {
        Task<IPhaseResponse> Init(IPhaseRequest request);
        Task<IPhaseResponse> Create(IPhaseRequest request);
        Task<IPhaseResponse> Approval(IPhaseRequest request);
        Task<IPhaseResponse> Pay(IPhaseRequest request);
    }

    public interface IPhaseRequest
    {
        public Loan LoanDetails { get; set; }
    }

    public interface IPhaseResponse
    {
        public Loan LoanDetails { get; set; }
    }

    public interface IPhaseFactory
    {
        Task<IPhaseResponse>
        GetPhaseAsync(IPhaseRequest request);
    }

}