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
        string RequestType { get; }
    }

    public interface IPhaseViewResponse
    {
        string ResponseType { get; }
    }

}