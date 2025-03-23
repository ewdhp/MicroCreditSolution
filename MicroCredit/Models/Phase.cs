using MicroCredit.Interfaces;

namespace MicroCredit.Models
{
    public class InitialRequest : IPhaseRequest
    {
        public CStatus Status { get; set; }
    }

    public class CreateRequest : IPhaseRequest
    {
        public CStatus Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class ApprovalRequest : IPhaseRequest
    {
        public CStatus Status { get; set; }
    }

    public class InitialResponse : IPhaseResponse
    {
        public bool Success { get; set; }
        public CStatus Status { get; set; }
    }

    public class CreateResponse : IPhaseResponse
    {
        public bool Success { get; set; }
        public CStatus Status { get; set; }

    }

    public class ApprovalResponse : IPhaseResponse
    {
        public bool Success { get; set; }
        public CStatus Status { get; set; }
    }

    public class PayResponse : IPhaseResponse
    {
        public bool Success { get; set; }
        public CStatus Status { get; set; }
    }
}