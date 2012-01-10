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
using System.Collections.Generic;

namespace Caliburn.Micro.Extensions
{
    /// <summary>
    /// An implementation of <see cref = "IResult" /> that implements navigation logic.
    /// </summary>
    /// <typeparam name="T">The type of the ViewModel navigated to</typeparam>
    public class NavigateResult<T> : IResult
        where T : class
    {
        private readonly Func<T> _targetDelegate;

        /// <summary>
        /// Delegate that activates the target ViewModel upon completion of the navigation logic.
        /// </summary>
        /// <remarks>The default implementation activates the target ViewModel using the conductor's ActivateItem method.</remarks>
        public Action<NavigateResult<T>> Activate = sender => sender.Conductor.ActivateItem(sender.Target);

        /// <summary>
        /// Delegate that evaluates whether the current active ViewModel allows to be navigated away from.
        /// </summary>
        /// <remarks>The default implementation attempts to call the CanClose method of the current active ViewModel.</remarks>
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
        /// Delegate that evaluates whether the target ViewModel allows to be navigated to.
        /// </summary>
        /// <remarks>No default implementation.</remarks>
        public Action<NavigateResult<T>, Action<bool>> CanNavigateTo;

        /// <summary>
        /// Delegate that allows for synchronous preparation of the target ViewModel, such as passing parameters.
        /// </summary>
        public Action<NavigateResult<T>> Prepare;

        /// <summary>
        /// Delegate that allows for asynchronous preperation of the target ViewModel.
        /// </summary>
        /// <returns>An implementation of IResult</returns>
        public Func<NavigateResult<T>, IResult> PrepareAsync;

        private T _target;

        /// <summary>
        /// Initializes a new instance of NavigateResult.
        /// </summary>
        /// <param name="conductor">The Caliburn Micro conductor used for the navigation.</param>
        /// <param name="targetDelegate">Delegate that evaluates to the target ViewModel instance.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NavigateResult(IConductActiveItem conductor, Func<T> targetDelegate)
        {
            if (conductor == null) throw new ArgumentNullException("conductor");
            if (targetDelegate == null) throw new ArgumentNullException("targetDelegate");

            _targetDelegate = targetDelegate;
            Conductor = conductor;
        }

        /// <summary>
        /// The current conductor used for the navigation.
        /// </summary>
        public IConductActiveItem Conductor { get; private set; }

        /// <summary>
        /// The target ViewModel instance.
        /// </summary>
        public T Target
        {
            get { return _target ?? (_target = _targetDelegate()); }
        }

        #region IResult Members

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ActionExecutionContext context)
        {
            IResult result = Navigate().ToSequential();
            result.Completed += (sender, args) => EventFns.RaiseOnce(ref Completed, this, args);
            result.Execute(context);
        }

        /// <summary>
        /// Event indication completion of the result.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

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
    }
}