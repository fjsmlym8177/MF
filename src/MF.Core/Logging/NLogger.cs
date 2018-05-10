using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Logging
{
    public class NLogger : ILogger
    {
        private static IDictionary<string, Logger> Loggers = new Dictionary<string, Logger>();



        private void InsertLog(string loggerName, LogLevel logLevel, string shortMessage, string fullMessage = "")
        {
            if (!Loggers.ContainsKey(loggerName))
                Loggers[loggerName] = LogManager.GetLogger(loggerName);

            var logger = Loggers[loggerName];
            switch (logLevel)
            {
                case LogLevel.Debug:
                    logger.Debug(shortMessage + Environment.NewLine + fullMessage);
                    break;
                case LogLevel.Information:
                    logger.Info(shortMessage + Environment.NewLine + fullMessage);
                    break;
                case LogLevel.Warning:
                    logger.Warn(shortMessage + Environment.NewLine + fullMessage);
                    break;
                case LogLevel.Error:
                    logger.Error(shortMessage + Environment.NewLine + fullMessage);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(shortMessage + Environment.NewLine + fullMessage);
                    break;
                default:
                    break;
            }
        }

        public void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "")
        {
            InsertLog("Null", logLevel, shortMessage, fullMessage);
        }

        public void InsertOtherLog(string loggerName, LogLevel logLevel, string shortMessage, string fullMessage = "")
        {
            InsertLog(loggerName, logLevel, shortMessage, fullMessage);
        }

        public void InsertQueueLog(string shortMessage, string fullMessage = "")
        {
            InsertLog("Queue", LogLevel.Information, shortMessage, fullMessage);
        }

        public void InsertDBLog(string shortMessage, string fullMessage = "")
        {
            InsertLog("Sql", LogLevel.Information, shortMessage, fullMessage);
        }

        public void InsertRequestLog(string shortMessage, string fullMessage = "")
        {
            InsertLog("Request", LogLevel.Information, shortMessage, fullMessage);
        }

        public void InsertHttpLog(string shortMessage, string fullMessage = "")
        {
            InsertLog("Http", LogLevel.Information, shortMessage, fullMessage);
        }
    }
}
