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

#if !SILVERLIGHT || SILVERLIGHT5
using System.Threading.Tasks;
#endif

namespace Cocktail
{
    /// <summary>
    /// An implementation of <see cref="IResult"/> providing information about the modal dialog or message box.
    /// </summary>
    public abstract class DialogOperationResult<T> : IResult
    {
        private ResultCompletionEventArgs _completionEventArgs;
#if !SILVERLIGHT || SILVERLIGHT5
        private TaskCompletionSource<T> _tcs;
#endif

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

#if !SILVERLIGHT || SILVERLIGHT5
        /// <summary>
        /// Returns a Task&lt;T&gt; for the current DialogOperationResult.
        /// </summary>
        public Task<T> AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<T>();
            Completed += (sender, args) =>
                             {
                                 if (args.WasCancelled)
                                     _tcs.TrySetCanceled();
                                 else if (args.Error != null)
                                     _tcs.TrySetException(args.Error);
                                 else
                                     _tcs.TrySetResult(args.Error == null ? DialogResult : default(T));
                             };

            return _tcs.Task;
        }

        /// <summary>
        /// Converts a DialogOperationResult&lt;T&gt; to a Task&lt;T&gt;.
        /// </summary>
        /// <param name="operation">The DialogOperationResult to be converted.</param>
        /// <returns>The converted Task.</returns>
        public static implicit operator Task<T>(DialogOperationResult<T> operation)
        {
            return operation.AsTask();
        }
#endif        
        
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