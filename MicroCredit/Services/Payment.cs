using System;
using System.Threading.Tasks;
using MicroCredit.Data;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Services
{
    public class PaymentService(
        IUCService user,
        UDbContext context) : IPaymentService
    {
        private readonly IUCService _user = user ??
        throw new ArgumentNullException(nameof(user));
        private readonly UDbContext _context = context ??
        throw new ArgumentNullException(nameof(context));

        public virtual Task<IPaymentDetails>
            Pay(PaymentRequest request)
        {
            // Implement the payment logic here
            throw new NotImplementedException();
        }
    }


    public class ExtendedPaymentService
        (UDbContext context, IUCService user) :
        PaymentService(user, context)
    {
        public override async
        Task<IPaymentDetails>
        Pay(PaymentRequest request)
        {
            // Implement the extended payment logic here
            // For example, you can call the base method and add additional logic
            var paymentDetails = await base.Pay(request);
            // Add additional logic if needed
            return paymentDetails;
        }
    }
}