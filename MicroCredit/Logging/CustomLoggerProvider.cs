using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MicroCredit.Logging
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly CustomLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, CustomLogger> _loggers = new ConcurrentDictionary<string, CustomLogger>();

        public CustomLoggerProvider(CustomLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new CustomLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}