using System.Threading.Tasks;
using MicroCredit.Models;

namespace MicroCredit.Interfaces
{
    public interface IPay
    {
        public IPayResponse Process(Loan data);
    }

    public interface IPayResponse
    {
        public bool Success { get; set; }
        public Loan Data { get; set; }
    }

}

