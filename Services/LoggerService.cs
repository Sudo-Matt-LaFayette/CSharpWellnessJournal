using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace DailyCheckInJournal.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService()
        {
            var logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DailyCheckInJournal",
                "Logs"
            );
            Directory.CreateDirectory(logDirectory);

            var logFile = Path.Combine(logDirectory, "app-.log");

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }
    }
}

