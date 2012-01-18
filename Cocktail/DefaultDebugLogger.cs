//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>Default Caliburn.Micro logger for debug builds.</summary>
    /// <remarks>A logger for diagnostic output from Caliburn.Micro.
    /// Typically activated in the static constructor of the AppBootstrapper as it is
    /// in Cocktail <see cref="FrameworkBootstrapper"/>.
    /// The diagnostics messages are sent to the <see cref="TraceLogger"/>.
    /// For diagnostics messages to be sent to the TraceLogger, the Cocktail debug assemblies must be used.
    /// </remarks>
    /// <example>
    /// 	<code title="" description="static AppBootstrapper(
    /// {
    ///      DefaultDebugLogger.SetAsLogger();
    /// }" lang="C#"></code>
    /// </example>
    public sealed class DefaultDebugLogger : ILog
    {
        private readonly Type _type;
        private readonly bool _isTypeWithSkippableNames;

        static DefaultDebugLogger()
        {
            SetDefaultSkippableNames();
        }

        /// <summary>Set the Caliburn Logger with Cocktail <see cref="Cocktail.DefaultDebugLogger"/>.</summary>
        [Conditional("DEBUG")]
        public static void SetAsLogger()
        {
            LogManager.GetLog = type => new DefaultDebugLogger(type);
        }

        /// <summary>Constructor for a type to log.</summary>
        /// <param name="type">The type of object that is being logged about.</param>
        public DefaultDebugLogger(Type type)
        {
            _type = type;
            _isTypeWithSkippableNames = _type == typeof(ViewModelBinder);
        }

        #region ILog Members

        void ILog.Info(string format, params object[] args)
        {
            if (MatchesSkippableName(args)) return;
            Log("INFO", format, args);
        }

        void ILog.Warn(string format, params object[] args)
        {
            Log("WARN", format, args);
        }

        void ILog.Error(Exception exception)
        {
            Log("ERROR", exception.ToString());
        }

        #endregion

        #region Log methods

        private void Log(string level, string format, params object[] args)
        {
            var msg = LogFormatter(_type, level, string.Format(format, args));
            LogWriter(msg);
        }

        /// <summary>
        /// Replaceable log message formatter.
        /// See <see cref="DefaultLogFormatter"/> example.
        /// </summary>
        public static Func<Type, string, string, string> LogFormatter = DefaultLogFormatter;

        /// <summary>Default formatter of log messages; see <see cref="Log"/>.</summary>
        public static string DefaultLogFormatter(Type sourceType, string level, string message)
        {
            return string.Format(
                "{0} {1}: {2}",
                sourceType.Name,
                level,
                message);
        }

        /// <summary>Writes log messages to <see cref="System.Diagnostics.Debug"/>.</summary>
        private static void LogWriter(string message)
        {
            DebugFns.WriteLine(message);
        }

        #endregion

        #region MatchesSkippableName

        ///<summary>
        /// Skip logging of message about routine and harmless ViewModel members.
        /// </summary>
        /// <remarks>
        /// Currently only skips if the logging type is 
        /// <see cref="Caliburn.Micro.ViewModelBinder"/>
        /// and the first arg is in <see cref="SkippableNames"/>.
        /// </remarks>
        private bool MatchesSkippableName(IEnumerable<object> args)
        {
            if (!_isTypeWithSkippableNames) return false;
            var nameArg = args.FirstOrDefault() as string;
            return null != nameArg && (SkippableNames.Any(x => nameArg == x));
        }

        /// <summary>
        /// Collection of type member names that should not be logged (should be skipped).
        /// </summary>
        public static ICollection<String> SkippableNames { get; set; }

        private static void SetDefaultSkippableNames()
        {
            // Glean public members from Caliburn.Micro.Conductor<T>
            // which derives from most of the base ViewModel classes you'll use.
            // Would be rare to want to log about these members.
            var typeWithSkippableMembers = typeof(Conductor<>);
            SkippableNames = typeWithSkippableMembers.GetMethods().Select(_ => _.Name).ToList();
            var names = typeWithSkippableMembers.GetProperties().Select(_ => _.Name);
            foreach (var name in names)
            {
                SkippableNames.Add("get_" + name);
                SkippableNames.Add("set_" + name);
            }
            names = typeWithSkippableMembers.GetEvents().Select(_ => _.Name);
            foreach (var name in names)
            {
                SkippableNames.Add("add_" + name);
                SkippableNames.Add("remove_" + name);
            }
        }

        #endregion
    }
}