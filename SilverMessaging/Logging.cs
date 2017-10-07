using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverPublish.Logging
{
    public interface ILogging
    {
        void Debug(object message, Exception t);
        void Info(object message, Exception t);
        void Warn(object message, Exception t);
        void Error(object message, Exception t);
        void Fatal(object message, Exception t);

    }
    public class LogMessage {
        public LogMessage(string method, string details, object context)
        {
            Context = context;
            Method = method;
            Details = details;
        }

        public string Details { get; set; }
        public string Method { get; set; }
        public object Context  { get; set; }
    }
}
