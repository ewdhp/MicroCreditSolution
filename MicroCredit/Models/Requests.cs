using System;
using MicroCredit.Interfaces;

namespace MicroCredit.Models
{

    public class InitialRequest : IPhaseReq
    {
        public decimal Amount { get; set; }
    }
    public class ApprovalRequest : IPhaseReq
    {
    }
    public class PayRequest : IPhaseReq
    {
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


    public class CreateLoanRequest
    {
        public double Amount { get; set; }
    }

    public class UpdateLoanStatusRequest
    {
        public int Status { get; set; }
    }

}