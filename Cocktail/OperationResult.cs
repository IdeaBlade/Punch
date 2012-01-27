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
        private INotifyCompletedArgs _args;

        /// <summary>Constructs a wrapper around the provided asynchronous function.</summary>
        /// <param name="asyncOp">The asynchronous DevForce function to be wrapped.</param>
        public OperationResult(INotifyCompleted asyncOp)
        {
            _asyncOp = asyncOp;
        }

        /// <summary>
        /// Returns whether the operation completed successfully.
        /// </summary>
        public bool CompletedSuccessfully
        {
            get { return _args != null && _args.Error == null; }
        }

        /// <summary>
        /// Returns whether the operation failed. 
        /// </summary>
        public bool HasError
        {
            get { return _args != null && _args.Error != null; }
        }

        /// <summary>
        /// Returns the exception if the operation failed. 
        /// </summary>
        public Exception Error
        {
            get { return _args != null ? _args.Error : null; }
        }

        /// <summary>
        /// Returns whether the operation was cancelled. 
        /// </summary>
        public bool Cancelled
        {
            get { return _args != null && _args.Cancelled; }
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
            _args = args;
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