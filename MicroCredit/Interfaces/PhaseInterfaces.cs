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
        public CStatus Status { get; set; }

    }

    public interface IPhaseRes
    {
        public CStatus Status { get; set; }
    }

    public interface IPhaseFactory
    {
        IPhase GetPhaseService(CStatus status);
    }

}