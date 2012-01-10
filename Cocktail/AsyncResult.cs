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
using Caliburn.Micro;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>An implementation of <see cref = "IResult" /> that wraps a DevForce asynchronous function.</summary>
    internal class AsyncResult : IResult
    {
        private readonly Func<INotifyCompleted> _asyncFunc;

        /// <summary>Constructs a wrapper around the provided asynchronous function.</summary>
        /// <param name="asyncFunc">The asynchronous DevForce function to be wrapped.</param>
        public AsyncResult(Func<INotifyCompleted> asyncFunc)
        {
            _asyncFunc = asyncFunc;
        }

        #region Implementation of IResult

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ActionExecutionContext context)
        {
            var op = _asyncFunc();
            op.WhenCompleted(OnComplete);
        }

        private void OnComplete(INotifyCompletedArgs args)
        {
            if (Completed == null) return;

            var resultArgs = new ResultCompletionEventArgs
                                 {
                                     Error = args.IsErrorHandled ? null : args.Error,
                                     WasCancelled = args.Cancelled
                                 };
            EventFns.RaiseOnce(ref Completed, this, resultArgs);
        }

        /// <summary>Signals the completion of the asynchronous operation.</summary>
        public event EventHandler<ResultCompletionEventArgs> Completed;

        #endregion
    }
}
