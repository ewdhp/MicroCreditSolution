using Microsoft.Extensions.Logging;
using System;

namespace MicroCredit.Logging
{
    public class LoggerCustomProvider : ILoggerProvider
    {
        private readonly LogLevel _minLogLevel;

        public LoggerCustomProvider(LogLevel minLogLevel)
        {
            _minLogLevel = minLogLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(_minLogLevel);
        }

        public void Dispose()
        {
        }

        private class CustomLogger : ILogger
        {
            private readonly LogLevel _minLogLevel;

            public CustomLogger(LogLevel minLogLevel)
            {
                _minLogLevel = minLogLevel;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= _minLogLevel;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId,
                TState state, Exception exception, Func<TState,
                Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                var color = Console.ForegroundColor;
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
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
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine($"{logLevel}: {formatter(state, exception)}");
                Console.ForegroundColor = color;
            }
        }
    }
}