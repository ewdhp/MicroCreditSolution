namespace MicroCredit.Interfaces
{
    public interface IPhase
    {
        bool Validate(IPhaseRequest request);
    }

    public interface IPhaseRequest
    {
        IPhase GetPhase();
    }
}