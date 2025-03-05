using System;
using System.Collections.Generic;

namespace MicroCredit.Models
{
    public class RNode
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public float Multiplier { get; set; }
        public float Earnings { get; set; }
        public string ReferralCode { get; set; }
        public string ReferrerId { get; set; }
        public List<RNode> Nodes { get; set; }
    }
}