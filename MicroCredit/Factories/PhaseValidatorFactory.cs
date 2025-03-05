using MicroCredit.Interfaces;
using MicroCredit.Models;
using Newtonsoft.Json.Linq;
using System;

namespace MicroCredit.Factories
{
    public static class PhaseRequestFactory
    {
        public static IPhaseRequest CreateRequest(JObject data)
        {
            var requestType = data["RequestType"]?.ToString();
            if (string.IsNullOrEmpty(requestType))
            {
                throw new ArgumentException("Request type is required");
            }

            switch (requestType)
            {
                case "LoanRequest":
                    return data.ToObject<LoanRequest>();
                case "ApprovalRequest":
                    return data.ToObject<ApprovalRequest>();
                case "DisburseRequest":
                    return data.ToObject<DisburseRequest>();
                default:
                    throw new ArgumentException("Invalid request type");
            }
        }
    }
}