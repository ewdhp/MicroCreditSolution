using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroCredit.Data;
using MicroCredit.Models;
using MicroCredit.Services;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Interfaces
{
    public interface IPhaseRequest
    {
        CStatus Status { get; set; }
    }
    public interface IPhaseResponse
    {
        public CStatus Status { get; set; }
    }
}