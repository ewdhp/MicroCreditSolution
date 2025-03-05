namespace MicroCredit.Interfaces
{
    public interface IPhase
    {
        bool Validate(IPhaseRequest request);
        bool IsValidTransition(int currentPhase);
    }

    public interface IPhaseRequest
    {
        string RequestType { get; }
        IPhase GetPhase();
    }
}