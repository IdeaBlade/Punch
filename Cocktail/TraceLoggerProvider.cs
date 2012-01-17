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
using System.Diagnostics;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>Provides DevForce with the current <see cref="ITraceLogger"/> implementation.</summary>
    /// <remarks>
    /// The default TraceLogger can be substituted with a custom TraceLogger by setting the TraceLogger delegate.
    /// </remarks>
    public class TraceLoggerProvider : ITraceLoggerProvider
    {
        /// <summary>
        /// Replaceable <see cref="ITraceLogger"/>.
        /// See <see cref="DefaultTraceLogger"/> example.
        /// </summary>
        public static Func<ITraceLogger> TraceLogger = DefaultTraceLogger;

        #region ITraceLoggerProvider Members

        /// <summary>
        /// Return the <see cref="T:IdeaBlade.Core.ITraceLogger"/> to use for logging of debug and trace messages.
        /// </summary>
        /// <returns/>
        ITraceLogger ITraceLoggerProvider.GetLogger()
        {
            return TraceLogger();
        }

        #endregion

        /// <summary>
        /// Default TraceLogger.
        /// </summary>
        /// <returns>Default implementation for <see cref="ITraceLogger"/></returns>
        public static ITraceLogger DefaultTraceLogger()
        {
            return new CocktailTraceLogger();
        }
    }

    /// <summary>Internal use.</summary>
    internal class CocktailTraceLogger : ITraceLogger
    {
        private readonly ITraceLogger _defaultTraceLogger;

        internal CocktailTraceLogger()
        {
            var defaultLoggerProvider = new DefaultLoggerProvider();
            _defaultTraceLogger = defaultLoggerProvider.GetLogger();
        }

        #region ITraceLogger Members

        /// <summary>
        /// Write a DevForce tracing message.
        /// </summary>
        /// <param name="message"/>
        /// <remarks>
        /// Calls originating from DevForce via DebugFns and TraceFns calls will pass a <see cref="T:IdeaBlade.Core.TraceMessage"/>
        ///             to the method.
        /// </remarks>
        public void Log(object message)
        {
            // Log to the default DevForce logger
            _defaultTraceLogger.Log(message);

            // For convenience, let's also log to the console.
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Whether logger can be called from multiple threads.
        /// </summary>
        /// <remarks>
        /// If single-threaded, calls to the Log method will be from a single thread.  If multi-threaded,
        ///             you must ensure that your logger is thread safe.
        /// </remarks>
        public bool IsSingleThreaded
        {
            get { return _defaultTraceLogger.IsSingleThreaded; }
        }

        #endregion
    }
}