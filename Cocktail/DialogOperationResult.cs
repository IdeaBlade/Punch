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
using System.ComponentModel;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// Represents an asynchronous operation handle to a dialog or message box.
    /// </summary>
    public abstract partial class DialogOperationResult<T> : IResult
    {
        private ResultCompletionEventArgs _completionEventArgs;

        /// <summary>
        /// Initializes and new instance of DialogOperationResult.
        /// </summary>
        protected DialogOperationResult()
        {
            Composition.BuildUp(this);
        }

        /// <summary>
        /// Returns the user's response to a dialog or message box.
        /// </summary>
        public abstract T DialogResult { get; }

        /// <summary>Indicates whether the dialog or message box has been cancelled.</summary>
        /// <value>Cancelled is set to true, if the user clicked the designated cancel button in response to the dialog or message box.</value>
        public abstract bool Cancelled { get; }

        /// <summary>
        /// Creates a continuation that executes when the target operation completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the operation completes. When run, the delegate will be passed the completed operation as an argument.</param>
        /// <returns>The target operation.</returns>
        /// <exception cref="ArgumentNullException">The continuationAction argument is null.</exception>
        public DialogOperationResult<T> ContinueWith(Action<DialogOperationResult<T>> continuationAction)
        {
            if (continuationAction == null) throw new ArgumentNullException("continuationAction");

            ((IResult) this).Completed += (sender, args) => continuationAction(this);
            return this;
        }

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

        #region Hide Object Members

        /// <summary>
        /// Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            // ReSharper disable BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
            // ReSharper restore BaseObjectEqualsIsObjectEquals
        }

        /// <summary>
        /// Hidden.
        /// </summary>
        // ReSharper disable NonReadonlyFieldInGetHashCode
        [EditorBrowsable(EditorBrowsableState.Never)]
        // ReSharper restore NonReadonlyFieldInGetHashCode
        public override int GetHashCode()
        {
            // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
            // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
        }

        /// <summary>
        /// Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

        #endregion
    }
}