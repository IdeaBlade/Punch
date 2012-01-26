//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Diagnostics;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>The central logger used by Cocktail, DevForce and Caliburn.Micro.</summary>
    /// <remarks>
    /// The logging can be customized by setting the LogWriter delegate to a custom LogWriter.
    /// </remarks>
    public sealed class TraceLogger : ITraceLogger
    {
        private static readonly ITraceLogger DefaultTraceLogger;

        /// <summary>Replaceable log writer.</summary>
        /// <remarks>See <see cref="DefaultLogWriter"/> for an example.</remarks>
        public static Action<object> LogWriter = DefaultLogWriter;

        static TraceLogger()
        {
            var defaultLoggerProvider = new DefaultLoggerProvider();
            DefaultTraceLogger = defaultLoggerProvider.GetLogger();
        }

        internal TraceLogger()
        {
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
        void ITraceLogger.Log(object message)
        {
            LogWriter(message);
        }

        /// <summary>
        /// Whether logger can be called from multiple threads.
        /// </summary>
        /// <remarks>
        /// If single-threaded, calls to the Log method will be from a single thread.  If multi-threaded,
        ///             you must ensure that your logger is thread safe.
        /// </remarks>
        bool ITraceLogger.IsSingleThreaded
        {
            get { return DefaultTraceLogger.IsSingleThreaded; }
        }

        #endregion

        /// <summary>
        /// Default log writer.
        /// </summary>
        /// <param name="message">
        /// Calls originating from DevForce via DebugFns and TraceFns calls will pass a <see cref="TraceMessage"/> to the method.
        /// </param>
        public static void DefaultLogWriter(object message)
        {
            // Log to the default DevForce logger
            DefaultTraceLogger.Log(message);

            // For convenience, let's also log to the console.
            Debug.WriteLine(message);
        }
    }

    /// <summary>Internal use."</summary>
    public sealed class TraceLoggerProvider : ITraceLoggerProvider
    {
        #region ITraceLoggerProvider Members

        /// <summary>
        /// Return the <see cref="T:IdeaBlade.Core.ITraceLogger"/> to use for logging of debug and trace messages.
        /// </summary>
        /// <returns/>
        ITraceLogger ITraceLoggerProvider.GetLogger()
        {
            return new TraceLogger();
        }

        #endregion
    }
}