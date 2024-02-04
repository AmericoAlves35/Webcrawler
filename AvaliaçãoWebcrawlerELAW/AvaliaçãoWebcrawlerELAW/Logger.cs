using System;
using System.IO;

namespace AvaliaçãoWebcrawlerELAW
{
    public class Logger
    {
        private readonly string logPath = @"C:\Users\Americo\Desktop\AtividadeElaw\Log";

        public Logger()
        {
            InitializeLoggingDirectory();
        }

        private void InitializeLoggingDirectory()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
        }

        public void LogInformation(string message)
        {
            LogMessage("INFO", message);
        }

        public void LogWarning(string message)
        {
            LogMessage("WARN", message);
        }

        public void LogError(string message)
        {
            LogMessage("ERROR", message);
        }

        private void LogMessage(string level, string message)
        {
            string logFile = Path.Combine(logPath, $"{DateTime.Now:yyyyMMdd}.log");

            string logEntry = $"{DateTime.Now:HH:mm:ss} [{level}] {message}";

            File.AppendAllText(logFile, logEntry + Environment.NewLine);
        }
    }
}
