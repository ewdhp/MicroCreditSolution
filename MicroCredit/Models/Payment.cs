using MicroCredit.Interfaces;

namespace MicroCredit.Models
{
    public enum PMEnum
    {
        Transfer,
        Online,
        InStore
    }

    public class Payment1 : IPaymentDetails
    {
        public PMEnum Method { get; set; }
    }
    public class Payment2 : IPaymentDetails
    {
        public PMEnum Method { get; set; }
    }
    public class Payment3 : IPaymentDetails
    {
        public PMEnum Method { get; set; }
    }

    public class PaymentResponse : IPaymentResponse
    {
        public IPaymentDetails Data { get; set; }
    }
    public class PaymentRequest : IPaymentRequest
    {
        public IPaymentDetails Data { get; set; }
    }
}

