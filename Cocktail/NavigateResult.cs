// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.Micro;

#if !SILVERLIGHT4
using System.Threading.Tasks;
#endif

namespace Cocktail
{
    /// <summary>
    ///   An implementation of <see cref="IResult" /> that implements navigation logic.
    /// </summary>
    /// <typeparam name="T"> The type of the ViewModel navigated to </typeparam>
    public class NavigateResult<T> : IResult
        where T : class
    {
        private enum Status
        {
            WaitingToRun,
            Running,
            RanToCompletion,
            Cancelled,
            Faulted
        };

        private Status _status = Status.WaitingToRun;
        private Exception _error;
        private readonly Func<T> _targetDelegate;
#if !SILVERLIGHT4
        private TaskCompletionSource<bool> _tcs;
#endif

        /// <summary>
        ///   Returns whether the navigation completed successfully.
        /// </summary>
        public bool CompletedSuccessfully
        {
            get { return _status == Status.RanToCompletion; }
        }

        /// <summary>
        ///   Returns whether the navigation was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get { return _status == Status.Cancelled; }
        }

        /// <summary>
        ///   Returns whether the navigation completed with an error.
        /// </summary>
        public bool HasError
        {
            get { return _status == Status.Faulted; }
        }

        /// <summary>
        ///   Returns the exception if the navigation completed with an error.
        /// </summary>
        public Exception Error
        {
            get { return _error; }
        }

        /// <summary>
        ///   Delegate that activates the target ViewModel upon completion of the navigation logic.
        /// </summary>
        /// <remarks>
        ///   The default implementation activates the target ViewModel using the conductor's ActivateItem method.
        /// </remarks>
        public Action<NavigateResult<T>> Activate = sender => sender.Conductor.ActivateItem(sender.Target);

        /// <summary>
        ///   Delegate that evaluates whether the current active ViewModel allows to be navigated away from.
        /// </summary>
        /// <remarks>
        ///   The default implementation attempts to call the CanClose method of the current active ViewModel.
        /// </remarks>
        public Action<NavigateResult<T>, Action<bool>> CanNavigateAway =
            (sender, callback) =>
                {
                    if (sender.Conductor.ActiveItem == null)
                    {
                        callback(true);
                        return;
                    }

                    var closeGuard = sender.Conductor.ActiveItem as IGuardClose;
                    if (closeGuard != null)
                        closeGuard.CanClose(callback);
                    else
                        callback(true);
                };

        /// <summary>
        ///   Delegate that evaluates whether the target ViewModel allows to be navigated to.
        /// </summary>
        /// <remarks>
        ///   No default implementation.
        /// </remarks>
        public Action<NavigateResult<T>, Action<bool>> CanNavigateTo;

        /// <summary>
        ///   Delegate that allows for synchronous preparation of the target ViewModel, such as passing parameters.
        /// </summary>
        public Action<NavigateResult<T>> Prepare;

        /// <summary>
        ///   Delegate that allows for asynchronous preparation of the target ViewModel.
        /// </summary>
        /// <returns> An implementation of IResult </returns>
        public Func<NavigateResult<T>, IResult> PrepareAsync;

        private T _target;

        /// <summary>
        ///   Initializes a new instance of NavigateResult.
        /// </summary>
        /// <param name="conductor"> The Caliburn Micro conductor used for the navigation. </param>
        /// <param name="targetDelegate"> Delegate that evaluates to the target ViewModel instance. </param>
        /// <exception cref="ArgumentNullException"></exception>
        public NavigateResult(IConductActiveItem conductor, Func<T> targetDelegate)
        {
            if (conductor == null) throw new ArgumentNullException("conductor");
            if (targetDelegate == null) throw new ArgumentNullException("targetDelegate");

            _targetDelegate = targetDelegate;
            Conductor = conductor;
        }

        /// <summary>
        ///   The current conductor used for the navigation.
        /// </summary>
        public IConductActiveItem Conductor { get; private set; }

        /// <summary>
        ///   The target ViewModel instance.
        /// </summary>
        public T Target
        {
            get { return _target ?? (_target = _targetDelegate()); }
        }

        /// <summary>
        ///   Executes the navigation when a yield return is not possible.
        /// </summary>
        /// <param name="callback"> Callback notifying the caller of the completion of the navigation </param>
        public void Go(Action<ResultCompletionEventArgs> callback = null)
        {
            if (_status != Status.WaitingToRun)
                throw new InvalidOperationException(StringResources.NavigationNotInValidState);

            if (callback != null)
                Completed += (sender, args) => callback(args);
            ((IResult) this).Execute(null);
        }

        /// <summary>
        ///   Creates a continuation that executes when the navigation completes.
        /// </summary>
        /// <param name="continuationAction"> An action to run when the navigation completes. When run, the delegate will be passed the completed navigation as an argument. </param>
        /// <returns> The navigation operation. </returns>
        /// <exception cref="ArgumentNullException">The continuationAction argument is null.</exception>
        public NavigateResult<T> ContinueWith(Action<NavigateResult<T>> continuationAction)
        {
            if (continuationAction == null) throw new ArgumentNullException("continuationAction");

            ((IResult) this).Completed += (sender, args) => continuationAction(this);
            return this;
        }

#if !SILVERLIGHT4
        /// <summary>
        ///   Returns a Task&lt;T&gt; for the current NavigateResult.
        /// </summary>
        public Task AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<bool>();
            ((IResult) this).Completed +=
                (sender, args) =>
                    {
                        if (args.WasCancelled)
                            _tcs.SetCanceled();
                        else if (args.Error != null)
                            _tcs.SetException(args.Error);
                        else
                            _tcs.SetResult(true);
                    };

            if (_status == Status.WaitingToRun)
                Go();

            return _tcs.Task;
        }

        /// <summary>
        ///   Implicitly converts the current DialogOperationResult to type <see cref="Task" />
        /// </summary>
        /// <param name="operation"> The DialogOperationResult to be converted. </param>
        public static implicit operator Task(NavigateResult<T> operation)
        {
            return operation.AsTask();
        }
#endif

        #region IResult Members

        /// <summary>
        ///   Executes the result using the specified context.
        /// </summary>
        /// <param name="context"> The context. </param>
        void IResult.Execute(ActionExecutionContext context)
        {
            if (_status != Status.WaitingToRun) return;

            var result = Navigate().ToSequentialResult();
            result.Completed += (sender, args) => OnCompleted(args);

            _status = Status.Running;
            result.Execute(context);
        }

        /// <summary>
        ///   Event indication completion of the result.
        /// </summary>
        event EventHandler<ResultCompletionEventArgs> IResult.Completed
        {
            add
            {
                if (_status == Status.RanToCompletion || _status == Status.Cancelled || _status == Status.Faulted)
                {
                    var args = new ResultCompletionEventArgs
                                   {
                                       Error = _status == Status.Faulted ? _error : null,
                                       WasCancelled = _status == Status.Cancelled
                                   };
                    value(this, args);
                }
                else
                    Completed += value;
            }
            remove { Completed -= value; }
        }

        #endregion

        private IEnumerable<IResult> Navigate()
        {
            if (CanNavigateTo != null)
                yield return new CallbackResult(this, CanNavigateTo);

            if (CanNavigateAway != null)
                yield return new CallbackResult(this, CanNavigateAway);

            if (PrepareAsync != null)
                yield return PrepareAsync(this);

            if (Prepare != null)
                Prepare(this);

            if (!Target.Equals(Conductor.ActiveItem))
                Activate(this);
        }

        private void OnCompleted(ResultCompletionEventArgs args)
        {
            if (args.WasCancelled)
                _status = Status.Cancelled;
            else if (args.Error != null)
            {
                _status = Status.Faulted;
                _error = args.Error;
            }
            else
                _status = Status.RanToCompletion;

            EventFns.RaiseOnce(ref Completed, this, args);
        }

        private event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        #region Nested type: CallbackResult

        private class CallbackResult : IResult
        {
            private readonly Action<NavigateResult<T>, Action<bool>> _booleanCallbackDelegate;
            private readonly NavigateResult<T> _sender;

            public CallbackResult(NavigateResult<T> sender,
                                  Action<NavigateResult<T>, Action<bool>> booleanCallbackDelegate)
            {
                if (sender == null) throw new ArgumentNullException("sender");
                if (booleanCallbackDelegate == null) throw new ArgumentNullException("booleanCallbackDelegate");

                _sender = sender;
                _booleanCallbackDelegate = booleanCallbackDelegate;
            }

            #region IResult Members

            public void Execute(ActionExecutionContext context)
            {
                _booleanCallbackDelegate(_sender,
                                         result =>
                                         EventFns.RaiseOnce(ref Completed, this,
                                                            new ResultCompletionEventArgs {WasCancelled = !result}));
            }

            public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

            #endregion
        }

        #endregion

        #region Hide Object Members

        /// <summary>
        ///   Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            // ReSharper disable BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
            // ReSharper restore BaseObjectEqualsIsObjectEquals
        }

        /// <summary>
        ///   Hidden.
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
        ///   Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        ///   Hidden.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

        #endregion
    }
}