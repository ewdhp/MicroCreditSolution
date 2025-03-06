using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class LoanResponse : IPhaseViewResponse
    {
        public string ResponseType { get; set; }
    }
    public class ApprovalResponse : IPhaseViewResponse
    {
        public string ResponseType { get; set; }

    }
    public class DisburseResponse : IPhaseViewResponse
    {
        public string ResponseType { get; set; }
    }

}