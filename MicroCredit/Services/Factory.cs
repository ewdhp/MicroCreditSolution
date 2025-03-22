using System;
using Microsoft.Extensions.DependencyInjection;
namespace MicroCredit.Services
{
    public interface IFactoryService
    {
        T Create<T>() where T : class;
    }
    public class FactoryService(IServiceProvider sp) : IFactoryService
    {
        private readonly IServiceProvider _sp = sp;

        public T Create<T>() where T : class
        {
            return _sp.GetRequiredService<T>();
        }
    }
}