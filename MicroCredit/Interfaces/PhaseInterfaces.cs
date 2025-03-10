using System;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{

    public interface IPhase
    {
        Task<(bool, IPhaseRes)> CompleteAsync(IPhaseReq request);
    }

    public interface IPhaseReq
    {
        public string Type { get; }
        public string Action { get; set; }

    }

    public interface IPhaseRes
    {
        public string Type { get; }
    }

    public interface IPhaseFactory
    {
        IPhase GetPhaseService(CStatus status);
    }

}