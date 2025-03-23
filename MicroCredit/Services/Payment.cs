using System;
using System.Threading.Tasks;
using MicroCredit.Data;
using MicroCredit.Interfaces;
using MicroCredit.Models;
using Microsoft.Extensions.Logging;

namespace MicroCredit.Services
{
    public class PayService(IUCService user, UDbContext dbContext)
    {
        protected readonly IUCService _user = user ??
        throw new ArgumentNullException(nameof(user));
        protected readonly UDbContext _dbContext = dbContext ??
        throw new ArgumentNullException(nameof(dbContext));
    }

  public class PayOnline : PayService, IPay
    {
        public PayOnline(IUCService u, UDbContext c) : base(u, c)
        {
        }

        public IPayResponse Process(Loan data)
        {
            throw new NotImplementedException();
        }
    }

    public class PayInStore : PayService, IPay
    {
        public PayInStore(IUCService u, UDbContext c) : base(u, c)
        {
        }

         public IPayResponse Process(Loan data)
        {
            throw new NotImplementedException();
        }
    }
}