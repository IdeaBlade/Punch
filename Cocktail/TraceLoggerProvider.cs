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

using System.Diagnostics;
using IdeaBlade.Core;

namespace Cocktail
{
#if !SILVERLIGHT
    [PartNotDiscoverable]
#endif
    /// <summary>Internal use.</summary>
    public class TraceLoggerProvider : ITraceLoggerProvider
    {
        /// <summary>
        /// Return the <see cref="T:IdeaBlade.Core.ITraceLogger"/> to use for logging of debug and trace messages.
        /// </summary>
        /// <returns/>
        public ITraceLogger GetLogger()
        {
            return new DebugConsoleLogger();
        }
    }

    /// <summary>Internal use.</summary>
    public class DebugConsoleLogger : ITraceLogger
    {
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
            get { return false; }
        }
    }
}
