using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Logging
{
    public interface ILogger
    {
        void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "");

        void InsertQueueLog(string shortMessage, string fullMessage = "");

        void InsertDBLog(string shortMessage, string fullMessage = "");

        void InsertHttpLog(string shortMessage, string fullMessage = "");

        void InsertRequestLog(string shortMessage, string fullMessage = "");

        void InsertOtherLog(string loggerName, LogLevel logLevel, string shortMessage, string fullMessage = "");
    }
}
