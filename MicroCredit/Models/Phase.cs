using MicroCredit.Interfaces;

namespace MicroCredit.Models
{
    public abstract class PhaseRequest : IPhaseRequest
    {
        public Loan Data { get; set; }
    }

    public class InitialRequest : PhaseRequest
    {
    }

    public class CreateRequest : PhaseRequest
    {
    }

    public class ApprovalRequest : PhaseRequest
    {
    }

    public class PayRequest : PhaseRequest
    {
    }

    public abstract class PhaseResponse : IPhaseResponse
    {
        public Loan Data { get; set; }
    }

    public class InitialResponse : PhaseResponse
    {
    }

    public class CreateResponse : PhaseResponse
    {
    }

    public class ApprovalResponse : PhaseResponse
    {
    }

    public class PayResponse : PhaseResponse
    {
    }
}