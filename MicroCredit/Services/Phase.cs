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
                var loan = await _loan
                .GetCurrentLoanAsync();
                if(loan == null)
                {
                    _logger.LogError("Loan not found.");    
                    return new PhaseResponse
                    {
                        Success = false,
                        Msg = "Loan not found."
                    };
                }

                var status = loan.Status;
                var paid = await _loan.AreAllPaidAsync();
                if(paid) status = CStatus.Pre; 
                var amount = (decimal)
                (((request?.Init?.Amount?? 0) == 0) ? 
                (decimal?)null : request.Init.Amount);

                _logger.LogInformation
                ("Phase status: {status}.",status);
                logger.LogInformation
                ("Phase amount: {amount}.",amount);

                return status switch
                {
                    CStatus.Pre => await Init(0),
                    CStatus.Initial => await Init(amount),
                    CStatus.Create => await Approval(request),
                    CStatus.Pending => await Approval(request),
                    CStatus.Rejected => await Approval(request),
                    CStatus.Active => await Pay(request),
                    CStatus.Due => await Pay(request),
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

        public async Task<PhaseResponse> 
        Init(decimal a)
        {
            _logger.LogInformation
            ("Init.Amount.{amount}",a); 
            PhaseResponse response = new();

            if((int)a == 0) return new 
                PhaseResponse 
                { 
                    Success = true,
                    Msg = "Pre",
                    Component = "TakeLoan",
                };

            try          
            {             
                var l = new Loan
                { 
                    Amount = (decimal)a, 
                    Status = CStatus.Create 
                };
               await _db.Loans.AddAsync(l);
               await _db.SaveChangesAsync(); 
               response.Success = true;
               response.LoanData = l;         
            }
            catch (Exception ex)
            {
                _logger.LogError
                (ex, "Error creating the loan.");
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

