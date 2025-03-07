using Microsoft.Extensions.Logging;
using System;

namespace MicroCredit.Logging
{
    public class LoggerCustom : ILogger
    {
        private readonly string _name;
        private readonly LogLevel _minLogLevel;

        public LoggerCustom(string name, LogLevel minLogLevel)
        {
            _name = name;
            _minLogLevel = minLogLevel;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var color = logLevel switch
            {
                LogLevel.Trace => "\u001b[37m", // White
                LogLevel.Debug => "\u001b[36m", // Cyan
                LogLevel.Information => "\u001b[36m",
                LogLevel.Warning => "\u001b[33m", // Yellow
                LogLevel.Error => "\u001b[31m", // Red
                LogLevel.Critical => "\u001b[35m", // Magenta
                _ => "\u001b[0m" // Reset
            };

            var logMessage = formatter(state, exception);
            Console.WriteLine($"{color}{logLevel}: {_name}[{eventId.Id}]: {logMessage}\u001b[0m");

            if (exception != null)
            {
                Console.WriteLine($"{color}{exception}\u001b[0m");
            }
        }

        // Custom log method for informational logs
        public void LogCustom(string message, params object[] args)
        {
            var color = "\u001b[32m"; // Green for custom informational logs
            var formattedMessage = string.Format(message, args);
            Console.WriteLine($"{color}Custom Information: {_name}: {formattedMessage}\u001b[0m");
        }
    }

    public class LoggerCustomProvider : ILoggerProvider
    {
        private readonly LogLevel _minLogLevel;

        public LoggerCustomProvider(LogLevel minLogLevel)
        {
            _minLogLevel = minLogLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerCustom(categoryName, _minLogLevel);
        }

        public void Dispose() { }
    }
}