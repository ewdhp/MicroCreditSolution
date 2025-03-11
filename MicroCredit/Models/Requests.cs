using System;
using MicroCredit.Interfaces;

namespace MicroCredit.Models
{

    public class InitialRequest : IPhaseReq
    {
        public CStatus Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class PendingRequest : IPhaseReq
    {
        public CStatus Status { get; set; }
    }
    public class ApprovalRequest : IPhaseReq
    {
        public CStatus Status { get; set; }
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