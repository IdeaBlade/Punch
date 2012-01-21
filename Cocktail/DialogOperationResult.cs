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

namespace Cocktail
{
    /// <summary>
    /// An implementation of <see cref="IResult"/> providing information about the modal dialog or message box.
    /// </summary>
    public abstract class DialogOperationResult<T> : IResult
    {
        private ResultCompletionEventArgs _completionEventArgs;

        /// <summary>
        /// Initializes and new instance of DialogOperationResult.
        /// </summary>
        protected DialogOperationResult()
        {
            IoC.BuildUp(this);
        }

        /// <summary>
        /// Returns the user's response to a dialog or message box.
        /// </summary>
        public abstract T DialogResult { get; }

        /// <summary>Indicates whether the dialog or message box has been cancelled.</summary>
        /// <value>Cancelled is set to true, if the user clicked the designated cancel button in response to the dialog or message box.</value>
        public abstract bool Cancelled { get; }

        #region IResult Members

        void IResult.Execute(ActionExecutionContext context)
        {
        }

        event EventHandler<ResultCompletionEventArgs> IResult.Completed
        {
            add
            {
                if (_completionEventArgs != null)
                    value(this, _completionEventArgs);
                else
                    Completed += value;
            }
            remove { Completed -= value; }
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="Completed"/> event.
        /// </summary>
        protected void OnCompleted(object sender, EventArgs e)
        {
            _completionEventArgs = new ResultCompletionEventArgs { WasCancelled = Cancelled };
            if (Completed != null)
                EventFns.RaiseOnce(ref Completed, this, _completionEventArgs);
        }

        private event EventHandler<ResultCompletionEventArgs> Completed;
    }
}