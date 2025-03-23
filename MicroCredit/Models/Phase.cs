using MicroCredit.Interfaces;

namespace MicroCredit.Models
{
    public class InitialRequest : IPhaseRequest
    {
        public CStatus Status { get; set; }
        public Loan Data { get; set; }
    }

    public class CreateRequest : IPhaseRequest
    {
        public Loan Data { get; set; }
    }

    public class ApprovalRequest : IPhaseRequest
    {
        public Loan Data { get; set; }
    }

    public class PayRequest : IPhaseResponse
    {
        public Loan Data { get; set; }
    }

    public class InitialResponse : IPhaseResponse
    {
        public Loan Data { get; set; }
    }

    public class CreateResponse : IPhaseResponse
    {
        public Loan Data { get; set; }
    }

    public class ApprovalResponse : IPhaseResponse
    {
        public Loan Data { get; set; }
    }

    public class PayResponse : IPhaseResponse
    {
        public Loan Data { get; set; }
    }
}