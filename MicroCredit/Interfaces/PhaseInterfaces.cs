using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface IPhase
    {
        Task<IPhaseRes> CompleteAsync(IPhaseReq request);
    }

    public interface IPhaseReq
    {
    }

    public interface IPhaseRes
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public Loan Loan { get; set; }
    }

    public interface IPhaseFactory
    {
        IPhase GetPhase(CStatus status);
    }

}