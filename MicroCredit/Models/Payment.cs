using MicroCredit.Services;
namespace MicroCredit.Models
{
    public enum PMEnum
    {
        Transfer,
        Online,
        InStore
    }
    public class PayOnlineResponse : IPayResponse
    {
        public bool Success { get; set; }
        public Loan Data { get; set; }
    }

    public class PayInStoreResponse : IPayResponse
    {
        public bool Success { get; set; }
        public Loan Data { get; set; }
    }
}

