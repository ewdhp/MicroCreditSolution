using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface IPaymentService
    {
        public Task<IPaymentDetails> Pay(PaymentRequest request);
    }

    public interface IPaymentDetails
    {
        public PMEnum Method { get; set; }
    }
    public interface IPaymentRequest
    {
        public IPaymentDetails Data { get; set; }
    }
    public interface IPaymentResponse
    {
        public IPaymentDetails Data { get; set; }
    }

}

