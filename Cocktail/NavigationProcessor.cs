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
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// Handles navigation from current ViewModel to a new target ViewModel.
    /// </summary>
    /// <typeparam name="T">The type of the ViewModel.</typeparam>
    public class NavigationProcessor<T>
        where T : class
    {
        /// <summary>
        ///   Delegate that activates the target ViewModel upon completion of the navigation logic.
        /// </summary>
        /// <remarks>
        ///   The default implementation activates the target ViewModel using the conductor's ActivateItem method.
        /// </remarks>
        internal Action<NavigationProcessor<T>> Activate = sender => sender.Conductor.ActivateItem(sender.Target);

        /// <summary>
        ///   Delegate that evaluates whether the current active ViewModel allows to be navigated away from.
        /// </summary>
        /// <remarks>
        ///   The default implementation attempts to call the CanClose method of the current active ViewModel.
        /// </remarks>
        internal Action<NavigationProcessor<T>, Action<bool>> CanNavigateAway =
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
        internal Action<NavigationProcessor<T>, Action<bool>> CanNavigateTo;

        /// <summary>
        ///   Delegate that allows for synchronous preparation of the target ViewModel, such as passing parameters.
        /// </summary>
        internal Action<NavigationProcessor<T>> Prepare;

        /// <summary>
        ///   Delegate that allows for asynchronous preparation of the target ViewModel.
        /// </summary>
        /// <returns> An asynchronous Task. </returns>
        internal Func<NavigationProcessor<T>, Task> PrepareAsync;

        private readonly Func<T> _targetDelegate;
        private T _target;

        /// <summary>
        ///   Internal use.
        /// </summary>
        internal NavigationProcessor(IConductActiveItem conductor, Func<T> targetDelegate)
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

        internal async Task NavigateAsync()
        {
            if (CanNavigateTo != null)
                await ExecuteAndWaitForCallbackAsync(CanNavigateTo);

            if (CanNavigateAway != null)
                await ExecuteAndWaitForCallbackAsync(CanNavigateAway);

            if (PrepareAsync != null)
                await PrepareAsync(this);

            if (Prepare != null)
                Prepare(this);

            if (!Target.Equals(Conductor.ActiveItem))
                Activate(this);
        }

        private Task ExecuteAndWaitForCallbackAsync(Action<NavigationProcessor<T>, Action<bool>> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                action(this, result =>
                {
                    if (result)
                        tcs.SetResult(true);
                    else
                        tcs.SetCanceled();
                });
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
            return tcs.Task;
        }

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