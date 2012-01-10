using System;
using System.Linq;
using IdeaBlade.Core;

namespace Caliburn.Micro.Extensions.Logging
{
    internal class CaliburnMicroDebugLogger : ILog
    {
        private readonly Type _type;

        public CaliburnMicroDebugLogger(Type type)
        {
            _type = type;
        }

        #region ILog Members

        public void Info(string format, params object[] args)
        {
            DebugFns.WriteLine(CreateLogMessage(format, "INFO", args));
        }

        public void Warn(string format, params object[] args)
        {
            DebugFns.WriteLine(CreateLogMessage(format, "WARN", args));
        }

        public void Error(Exception exception)
        {
            DebugFns.WriteLine(CreateLogMessage(exception.ToString(), "INFO"));
        }

        #endregion

        private string CreateLogMessage(string format, string level, params object[] args)
        {
            return string.Format("[{0}] {1}: {2}", _type.Name.Split('.').Last(), level,
                                 string.Format(format, args));
        }
    }
}