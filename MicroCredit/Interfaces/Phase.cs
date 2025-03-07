using System;
using System.Threading.Tasks;

namespace MicroCredit.Interfaces
{

    public interface IPhase
    {
        public IPhaseViewResponse GetPhaseView();
        public Task<bool> ValidatePhase(IPhaseRequest request, string userId);
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