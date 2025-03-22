using MicroCredit.Interfaces;

namespace MicroCredit.Models
{
    public class InitialRequest : IPhaseRequest
    {
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
    public class PayRequest : IPhaseRequest
    {
        public Loan Data { get; set; }
    }

    public class Initial : IPhaseResponse
    {
        public Loan Data { get; set; }
    }
    public class Create : IPhaseResponse
    {
        public Loan Data { get; set; }
    }
    public class Approval : IPhaseResponse
    {
        public Loan Data { get; set; }
    }
    public class Pay : IPhaseResponse
    {

        public Loan Data { get; set; }
    }
}