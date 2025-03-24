using MicroCredit.Interfaces;
namespace MicroCredit.Models
{   
    public class PhaseResponse
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public string Component { get; set; }
        public Loan LoanData { get; set; }
    }

    public class PhaseRequest
    {
        public InitialRequest Init { get; set; }
        public ApprovalRequest Approval { get; set; }
        public PayRequest Pay { get; set; }
    }

    public class InitialRequest
    {
        public decimal Amount { get; set; }
    }
    public class CreateRequest{}
    public class ApprovalRequest{}
    public class PayRequest
    {
        public string Method { get; set; }
    }
}