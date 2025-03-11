using MicroCredit.Interfaces;
using MicroCredit.Models;
using System;

namespace MicroCredit.Models
{
    public class InitialResponse : IPhaseRes
    {
        public CStatus Status { get; set; }
    }
    public class PendingResponse : IPhaseRes
    {
        public CStatus Status { get; set; }
    }
    public class ApprovalResponse : IPhaseRes
    {
        public CStatus Status { get; set; }

    }

}