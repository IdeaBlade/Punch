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
    /// <summary>
    /// Encapsulates a DevForce asynchronous operation, that can interchangeably be  used in places  
    /// where an <see cref = "IResult" /> or <see cref="INotifyCompleted" /> is expected.
    /// </summary>
    /// <seealso cref="CoroutineFns.AsOperationResult"/>
    public class OperationResult : IResult, INotifyCompleted
    {
        private readonly INotifyCompleted _asyncOp;

        /// <summary>Constructs a wrapper around the provided asynchronous function.</summary>
        /// <param name="asyncOp">The asynchronous DevForce function to be wrapped.</param>
        public OperationResult(INotifyCompleted asyncOp)
        {
            _asyncOp = asyncOp;
        }

        #region Implementation of IResult

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void IResult.Execute(ActionExecutionContext context)
        {
            //var op = _asyncFunc();
            _asyncOp.WhenCompleted(OnComplete);
        }

        /// <summary>Signals the completion of the asynchronous operation.</summary>
        event EventHandler<ResultCompletionEventArgs> IResult.Completed
        {
            add { Completed += value; }
            remove { Completed -= value; }
        }

        #endregion

        #region INotifyCompleted Members

        void INotifyCompleted.WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            _asyncOp.WhenCompleted(completedAction);
        }

        #endregion

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

        private event EventHandler<ResultCompletionEventArgs> Completed;
    }
}