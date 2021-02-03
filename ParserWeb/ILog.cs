using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserWeb
{
    public enum LoggerMessageType { Trace, Info, Warn, Debug, Error };
    public delegate void DelegateEventLog(LoggerMessageType messageType, string message);
    public interface ILog
    {
        void Info(string message);
        void Error(string message);
        void Error(Exception exception, string message);
        void Error(Exception exception);

        void Warn(string message);
        void Debug(string message);

        event DelegateEventLog EventMessage;
    }
}
