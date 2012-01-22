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
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// An <see cref="INotifyCompleted"/> implementation that is
    /// completed immediately upon construction.
    /// </summary>
    /// <remarks>
    /// Useful when you need to bail out immediately (synchronously) 
    /// from a method that is normally asynchronous and long running.
    /// You control its <see cref="INotifyCompletedArgs"/> values via
    /// the constructor.
    /// Use <see cref="CoroutineFns.AsOperationResult"/> to present as an
    /// <see cref="OperationResult"/> to a Caliburn Coroutine.
    /// </remarks>
    public class NotifyCompletedImmediately : INotifyCompleted, INotifyCompletedArgs
    {
        private readonly NotifyCompletedImmediatelyArgs _args;

        /// <summary>
        /// Create a new <see cref="INotifyCompleted"/> instance that
        /// has already completed and is configured to your needs.
        /// See member properties corresponding to parameters.
        /// </summary>
        public NotifyCompletedImmediately(
            Exception error, bool cancelled = false, bool isErrorHandled = false)
        {
            _args = new NotifyCompletedImmediatelyArgs(
                error,
                (null != error) && isErrorHandled,
                cancelled);
        }

        /// <summary>
        /// Exception representing an error condition (if any); 
        /// default is null meaning "everything ok".
        /// </summary>
        public Exception Error { get { return _args.Error; } }

        /// <summary>
        /// True if <see cref="Error"/> should be considered "handled"; false by default.
        /// </summary>
        public bool IsErrorHandled
        {
            get { return _args.IsErrorHandled; }
            set { _args.IsErrorHandled = value; }
        }

        /// <summary>
        /// True if representing a cancelled operation; false by default.
        /// </summary>
        public bool Cancelled { get { return _args.Cancelled; } }

        void INotifyCompleted.WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            completedAction(_args);
        }

        private class NotifyCompletedImmediatelyArgs : INotifyCompletedArgs
        {
            public NotifyCompletedImmediatelyArgs(
                Exception error = null, bool cancelled = false, bool isErrorHandled = false)
            {
                Error = error;
                Cancelled = cancelled;
                IsErrorHandled = isErrorHandled;
            }

            public Exception Error { get; private set; }

            public bool Cancelled { get; private set; }

            public bool IsErrorHandled { get; set; }
        }
    }
}