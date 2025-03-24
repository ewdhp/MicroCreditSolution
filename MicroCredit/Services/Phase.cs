using System;
using MicroCredit.Data;
using MicroCredit.Models;
using System.Threading.Tasks;
using MicroCredit.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using System.Runtime;

namespace MicroCredit.Services
{
    public class PhaseService(
        IUCService userCS,
        UDbContext dbContext,
        ILoanService loanService,
        ILogger<PhaseService> logger)
    {
        private readonly UDbContext _db = dbContext ?? 
        throw new ArgumentNullException(nameof(dbContext));
        private readonly IUCService _user = userCS ?? 
        throw new ArgumentNullException(nameof(userCS));
        private readonly ILoanService _loan = loanService ?? 
        throw new ArgumentNullException(nameof(loanService));
        private readonly ILogger<PhaseService> _logger = logger ?? 
        throw new ArgumentNullException(nameof(logger));

    public async Task<PhaseResponse> 
    GetPhaseAsync(PhaseRequest request)
    {
        if (request == null)
        {
            _logger.LogError("Phase request is null.");              
            return new PhaseResponse
            {
                Success = false,
                Msg = "Phase request is null."
            };                
        }
        var currentUser = _user.GetUserId();
        if (currentUser == Guid.Empty)
        {
            _logger.LogError("User not found.");   
            return new PhaseResponse
            {
                Success = false,
                Msg = "User not found."
            };
        }
        try
        {
            var loan = await _loan.GetCurrentLoanAsync();
            var paid = await _loan.AreAllPaidAsync();               
            var amount = 0;
            CStatus status = CStatus.Pre;             
            if(!paid && loan != null)
            {
                status= loan.Status;
                amount= (int)loan.Amount;
            }
            return status switch
            {
                CStatus.Pre => await Init(0),
                CStatus.Initial => await Init(amount),
                CStatus.Create => await Approval(request),
                CStatus.Pending => await Approval(request),
                CStatus.Active => await Pay(request),
                CStatus.Due => await Pay(request),
                CStatus.Rejected => await Init(0),
                _ => throw new
                    ArgumentOutOfRangeException
                    (nameof(status), status, null)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError
            (ex, "Error GetPhaseAsync.");
            
            throw;
        }
    }

    public async Task<PhaseResponse> Init(int amount)
    {
        _logger.LogInformation
        ("Init.Amount.{amount}",
        amount);
            PhaseResponse response = new();
                if((int)amount == 0) return new
                    PhaseResponse 
                    { 
                        Success = true,
                        Msg = "Pre",
                        Component = "TakeLoan",
                        LoanData = new Loan()
                        {Status = CStatus.Initial}
                    };
            try          
            {             
                var l = new Loan
                {   Amount = (decimal)amount,
                    Status = CStatus.Create 
                };
                await _db.Loans.AddAsync(l);
                await _db.SaveChangesAsync(); 
                    response.Success = true;
                    response.Msg = "Loan created.";
                    response.Component = "LoanInfo";
                    response.LoanData = l;
                    _logger.LogInformation("Loan created");        
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Loan.");
            }
        return response;    
    }

        public async Task<PhaseResponse> 
        Create(PhaseRequest request)
        {
            throw new NotImplementedException();
        }
       
        private async Task<PhaseResponse> 
        Approval(PhaseRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task<PhaseResponse> 
        Pay(PhaseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

