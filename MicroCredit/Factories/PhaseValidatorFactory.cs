using MicroCredit.Interfaces;
using System;
using System.Collections.Generic;
using MicroCredit.Services;
using MicroCredit.Models;

namespace MicroCredit.Factories
{
    public static class PhaseValidatorFactory
    {
        private static readonly Dictionary<Type, Type> ValidatorMap = new Dictionary<Type, Type>
        {
            { typeof(LoanRequest), typeof(LoanPhase) },
            { typeof(ApprovalRequest), typeof(ApprovalPhase) },
            { typeof(DisburseRequest), typeof(DisbursePhase) }
        };

        public static IPhase CreateValidator(IPhaseRequest request)
        {
            var requestType = request.GetType();
            if (ValidatorMap.TryGetValue(requestType, out var validatorType))
            {
                return (IPhase)Activator.CreateInstance(validatorType);
            }
            throw new ArgumentException("Invalid request type");
        }
    }
}