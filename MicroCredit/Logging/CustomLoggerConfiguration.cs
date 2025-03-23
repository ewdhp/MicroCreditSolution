using Microsoft.Extensions.Logging;

namespace MicroCredit.Logging
{
    public class CustomLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}