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
using System.Reflection;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>
    /// A collection of static methods used to provide code assertions, tracing and logging functionality.
    /// </summary>
    /// <remarks><see cref="TraceLogger.LogWriter"/> can be used to redirect the output.</remarks>
    public static class LogFns
    {
        /// <summary>
        /// Write a string representation of an object to the log for debug builds of Cocktail applications.
        /// </summary>
        /// <param name="aObject">The object to be written to the log.</param>
        [Conditional("DEBUG")]
        public static void DebugWriteLine(object aObject)
        {
            DebugFns.WriteLine(FormatLogMessage(aObject));
        }

        /// <summary>
        /// Write a string representation of an object to the log for debug builds of Cocktail if the specified condition is true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="aObject">The object to be written to the log.</param>
        [Conditional("DEBUG")]
        public static void DebugWriteLineIf(bool condition, object aObject)
        {
            DebugFns.WriteLineIf(condition, FormatLogMessage(aObject));
        }

        /// <summary>
        /// Write a string representation of an object to the log.
        /// </summary>
        /// <param name="aObject">The object to be written to the log.</param>
        [Conditional("TRACE")]
        public static void TraceWriteLine(object aObject)
        {
            TraceFns.WriteLine(FormatLogMessage(aObject));
        }

        /// <summary>
        /// Write a string representation of an object to the log if the specified condition is true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="aObject">The object to be written to the log.</param>
        [Conditional("TRACE")]
        public static void TraceWriteLineIf(bool condition, object aObject)
        {
            TraceFns.WriteLineIf(condition, FormatLogMessage(aObject));
        }

        private static string FormatLogMessage(object aObject)
        {
            var stackFrame = new StackFrame(2);
            var method = stackFrame.GetMethod();

            var source = method == null ? "(Unknown)" : GetFullyQualifiedMethodName(method);
            return string.Format("{0}: {1}", source, aObject);
        }

        private static String GetFullyQualifiedMethodName(MemberInfo pMemberInfo)
        {
            if (pMemberInfo == null)
            {
                throw new ArgumentNullException("pMemberInfo");
            }
            if (pMemberInfo.DeclaringType != null)
            {
                return pMemberInfo.DeclaringType.FullName + ":" + pMemberInfo.Name;
            }
            return ":" + pMemberInfo.Name;
        }
    }
}