using System;
using System.Threading.Tasks;

namespace MicroCredit.Interfaces
{

    public interface IPhase
    {
        Task<bool> ProcessPhase(IPhaseRequest request);
    }

    public interface IPhaseRequest
    {
        public string Type { get; }
        public string Action { get; set; }

    }

    public interface IPhaseViewResponse
    {
        public string Type { get; }
    }

}