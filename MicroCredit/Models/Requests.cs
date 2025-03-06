using MicroCredit.Interfaces;
using MicroCredit.Services;

namespace MicroCredit.Models
{
    public class LoanRequest : IPhaseRequest
    {
        public string RequestType => "LoanRequest";
    }
    public class ApprovalRequest : IPhaseRequest
    {
        public string RequestType => "ApprovalRequest";

    }
    public class DisburseRequest : IPhaseRequest
    {
        public string RequestType => "DisburseRequest";
    }
    public class PhaseResetRequest
    {
        public int Phase { get; set; }
    }
    public class GetReferrerIdByCodeRequest
    {
        public string Code { get; set; }
    }
    public class GetReferrerIdByCodeResponse
    {
        public string ReferrerId { get; set; }
    }
    public class LinkReferralRequest
    {
        public string Id { get; set; }
        public string Code { get; set; }
    }
}