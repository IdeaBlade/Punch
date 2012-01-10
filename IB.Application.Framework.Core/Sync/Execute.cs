//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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
using System.Windows;
using System.Windows.Threading;

namespace IdeaBlade.Application.Framework.Core.Sync
{
    /// <summary>
    /// Enables easy marshalling of code to the UI thread.
    /// </summary>
    internal static class Execute
    {
        private static Action<Action> _executor = action => action();

        /// <summary>
        /// Initializes the framework using the current dispatcher.
        /// </summary>
        public static void InitializeWithDispatcher()
        {
#if SILVERLIGHT
            var dispatcher = Deployment.Current.Dispatcher;
#else
            var dispatcher = Dispatcher.CurrentDispatcher;
#endif
            _executor = action =>
            {
                if (dispatcher.CheckAccess())
                    action();
                else dispatcher.BeginInvoke(action);
            };
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void OnUIThread(this Action action)
        {
            _executor(action);
        }
    }
}