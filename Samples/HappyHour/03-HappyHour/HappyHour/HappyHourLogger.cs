using System;

namespace HappyHour
{
    public class HappyHourLogger : Caliburn.Micro.ILog
    {
        public static void SetAsLogger()
        {
            if (!System.Diagnostics.Debugger.IsLogging()) return; // Skip if not debugging
            Caliburn.Micro.LogManager.GetLog = type => new HappyHourLogger(type);
        }

        private readonly Type _type;

        public HappyHourLogger(Type type)
        {
            _type = type;
        }

        #region ILog Members

        public void Info(string format, params object[] args)
        {
            Log("INFO", format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log("WARN", format, args);
        }

        public void Error(Exception exception)
        {
            Log("ERROR", exception.ToString());
        }

        #endregion

        protected virtual void Log(string level, string format, params object[] args)
        {
            var msg = string.Format(
                "[{0}] {1} {2}: {3}",
                _logCounter++,
                 _type.Name,
                 level,
                 string.Format(format, args));

            LogWriter(msg);
        }
        private static int _logCounter = 1;

        protected virtual void LogWriter(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
