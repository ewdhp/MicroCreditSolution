using Microsoft.Extensions.Logging;
using System;

namespace MicroCredit.Logging
{
    public class CustomLogger : ILogger
    {
        private readonly string _name;
        private readonly CustomLoggerConfiguration _config;

        public CustomLogger(string name, CustomLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _config.LogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logRecord = $"{logLevel}: {_name} - {formatter(state, exception)}";

            // Change console text color based on log level
            var originalColor = Console.ForegroundColor;
            switch (logLevel)
            {
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ForegroundColor = originalColor;
                    break;
            }

            Console.WriteLine(logRecord);
            Console.ForegroundColor = originalColor; // Reset to original color
        }
    }
}