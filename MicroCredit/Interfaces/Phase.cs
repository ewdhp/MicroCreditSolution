using System;

namespace MicroCredit.Interfaces
{

    public interface IPhase
    {
        IPhaseViewResponse GetPhaseView();
        bool ValidatePhase(IPhaseRequest request);
    }

    public interface IPhaseRequest
    {
        public string Type { get; }
        public string Action { get; set; }

    }

    public interface IPhaseViewResponse
    {
        public string Type { get; }
    }

}