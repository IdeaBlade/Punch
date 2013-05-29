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
using System.Threading.Tasks;

namespace Cocktail
{
    /// <summary>
    ///     Provides data for the <see cref="INavigationTarget.OnNavigatingFrom" /> callback that can be used to cancel a navigation request.
    /// </summary>
    public class NavigationCancelArgs
    {
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        ///     Specifies whether a pending navigation should be canceled.
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                return Task.IsCanceled;
            }
        }

        /// <summary>
        ///     Cancel the current navigation request.
        /// </summary>
        public void Cancel()
        {
            EnsureTaskCompletionSource();
            _tcs.SetCanceled();
        }

        /// <summary>
        /// Defers continuation of the current navigation until one of the following methods is called: <see cref="Complete"/>, <see cref="Fail"/>, <see cref="Cancel"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if Defer has previously been called or the navigation no longer allows deferal.</exception>
        public void Defer()
        {
            if (_tcs != null)
                throw new InvalidOperationException("The state of the current navigation no longer allows deferal.");

            EnsureTaskCompletionSource();
        }

        /// <summary>
        /// Signals to the current navigation that it can continue successfully.
        /// </summary>
        public void Complete()
        {
            EnsureTaskCompletionSource();
            _tcs.SetResult(true);
        }

        /// <summary>
        /// Signals to the current navigation that it must continue with the provided error.
        /// </summary>
        /// <param name="error">The reason why the navigation failed.</param>
        public void Fail(Exception error)
        {
            EnsureTaskCompletionSource();
            _tcs.SetException(error);
        }

        internal Task Task
        {
            get { return _tcs != null ? _tcs.Task : TaskFns.FromResult(true); }
        }

        private void EnsureTaskCompletionSource()
        {
            if (_tcs == null)
                _tcs = new TaskCompletionSource<bool>();
        }
    }

    /// <summary>
    ///     Provides data for navigation methods that cannot cancel the navigation request.
    /// </summary>
    public class NavigationArgs : NavigationCancelArgs
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="parameter">Parameter passed to the target view model.</param>
        public NavigationArgs(object parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        ///     Gets the parameter passed to the target view model.
        /// </summary>
        public object Parameter { get; private set; }
    }

    /// <summary>
    ///     An optional interface for a view model to add code that responds to navigation events.
    /// </summary>
    public interface INavigationTarget
    {
        /// <summary>
        ///     Invoked when the view model becomes the current active view model at the end of a navigation request.
        /// </summary>
        /// <param name="args">Data relating to the pending navigation request.</param>
        void OnNavigatedTo(NavigationArgs args);

        /// <summary>
        ///     Invoked immediately before the view model is deactivated and is no longer the active view model due to a navigation request.
        /// </summary>
        /// <param name="args">Data relating to the pending navigation request.</param>
        void OnNavigatingFrom(NavigationCancelArgs args);

        /// <summary>
        ///     Invoked immediatly after the view model is deactivated and is no longer the active view model due to a navigation request.
        /// </summary>
        /// <param name="args"></param>
        void OnNavigatedFrom(NavigationArgs args);
    }

    internal static class NavigationTargetHelper
    {
        internal static async Task OnNavigatedToAsync(this INavigationTarget target, object parameter)
        {
            if (target == null) return;

            var args = new NavigationArgs(parameter);
            target.OnNavigatedTo(args);
            await args.Task;
        }

        internal static async Task OnNavigatingFromAsync(this INavigationTarget target)
        {
            if (target == null) return;

            var args = new NavigationCancelArgs();
            target.OnNavigatingFrom(args);
            await args.Task;
        }

        internal static async Task OnNavigatedFromAsync(this INavigationTarget target, object parameter)
        {
            if (target == null) return;

            var args = new NavigationArgs(parameter);
            target.OnNavigatedFrom(args);
            await args.Task;
        }
    }
}