using System;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{

    public interface IPhase
    {
        Task<(bool, IPhaseResponse)>
        CompleteAsync(IPhaseRequest request);
    }

    public interface IPhaseRequest
    {
        public string Type { get; }
        public string Action { get; set; }

    }

    public interface IPhaseResponse
    {
        public string Type { get; }
    }

    public interface IPhaseFactory
    {
        IPhase GetPhaseService(CStatus status);
    }

}