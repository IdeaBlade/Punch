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
using Caliburn.Micro;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cocktail
{
    public partial interface INavigatorConfigurator
    {
        /// <summary>
        ///   Configures the guard for the target type.
        /// </summary>
        /// <param name="guard"> The target guard. </param>
        /// <remarks>
        ///   The target guard controls whether the target type is an authorized navigation target.
        /// </remarks>
        INavigatorConfigurator WithTargetGuard(Func<Type, Task<bool>> guard);
    }

    public partial interface INavigator
    {
        /// <summary>
        ///   Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        ///   Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        ///   Navigates to the most recent item in forward navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        Task GoForwardAsync();

        /// <summary>
        ///   Navigates to the most recent item in back navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        Task GoBackAsync();
    }

    public partial class Navigator
    {
        private readonly IConductActiveItem _conductor;
        private readonly Frame _frame;
        private readonly FrameAdapterFix _frameAdapter;
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        ///   Initializes a new NavigationService for ViewModel-based navigation.
        /// </summary>
        /// <param name="conductor"> The underlying screen conductor used to activate navigation targets. </param>
        public Navigator(IConductActiveItem conductor)
        {
            if (conductor == null) throw new ArgumentNullException("conductor");

            _conductor = conductor;
        }

        /// <summary>
        ///   Initializes a new NavigationService for view-based navigation.
        /// </summary>
        /// <param name="frame"> The content control that supports navigation. </param>
        /// <param name="treatViewAsLoaded"> Treats the view as loaded if set to true. </param>
        public Navigator(Frame frame, bool treatViewAsLoaded = false)
        {
            if (frame == null) throw new ArgumentNullException("frame");

            _frame = frame;
            _frameAdapter = new FrameAdapterFix(_frame, treatViewAsLoaded);
            _frameAdapter.Navigated += OnNavigated;
            _frameAdapter.NavigationFailed += OnNavigationFailed;
            _frameAdapter.NavigationStopped += OnNavigationStopped;
        }

        #region INavigator Members

        /// <summary>
        ///   Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// </summary>
        public bool CanGoForward
        {
            get { return _frameAdapter != null && _frameAdapter.CanGoForward; }
        }

        /// <summary>
        ///   Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        public bool CanGoBack
        {
            get { return _frameAdapter != null && _frameAdapter.CanGoBack; }
        }

        /// <summary>
        ///   Navigates to the most recent item in forward navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        public Task GoForwardAsync()
        {
            if (_frameAdapter == null)
                throw new NotSupportedException(StringResources.NavigationServiceDoesNotManageHistory);

            return GoForwardWithFrame();
        }

        /// <summary>
        ///   Navigates to the most recent item in back navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        public Task GoBackAsync()
        {
            if (_frameAdapter == null)
                throw new NotSupportedException(StringResources.NavigationServiceDoesNotManageHistory);

            return GoBackWithFrame();
        }

        /// <summary>
        ///   Returns the current active ViewModel or null.
        /// </summary>
        public object ActiveViewModel
        {
            get
            {
                if (_conductor != null)
                    return _conductor.ActiveItem;

                var view = _frame.Content as FrameworkElement;
                if (view == null)
                    return null;

                return view.DataContext;
            }
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync(Type viewModelType, Func<object, Task> prepare)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (prepare == null) throw new ArgumentNullException("prepare");

            if (_conductor != null)
                return NavigateWithConductor(viewModelType, prepare);

            return NavigateWithFrame(viewModelType, prepare);
        }

        #endregion

        private async Task<bool> GuardAsync(Type viewModelType)
        {
            if (!await CanCloseAsync())
                return false;

            if (!await AuthorizeTargetAsync(viewModelType))
                return false;

            return true;
        }

        private async Task NavigateWithConductor(Type viewModelType, Func<object, Task> prepare)
        {
            if (!await GuardAsync(viewModelType))
                throw new TaskCanceledException();

            var target = Composition.GetInstance(viewModelType, null);
            await prepare(target);

            if (!ReferenceEquals(ActiveViewModel, target))
                _conductor.ActivateItem(target);
        }

        private async Task NavigateWithFrame(Type viewModelType, Func<object, Task> prepare)
        {
            if (_tcs != null)
                throw new InvalidOperationException(StringResources.PendingNavigation);

            try
            {
                var viewType = ViewLocator.LocateTypeForModelType(viewModelType, null, null);
                if (viewType == null)
                    throw new Exception(string.Format(StringResources.ViewNotFound, viewModelType.FullName));

                if (!await GuardAsync(viewModelType))
                    throw new TaskCanceledException();

                _tcs = new TaskCompletionSource<bool>();
                if (!_frameAdapter.Navigate(viewType, prepare))
                    _tcs.TrySetCanceled();

                await _tcs.Task;
            }
            finally
            {
                _tcs = null;
            }
        }

        private async Task GoForwardWithFrame()
        {
            if (_tcs != null)
                throw new InvalidOperationException(StringResources.PendingNavigation);

            try
            {
                if (!await CanCloseAsync())
                    throw new TaskCanceledException();

                _tcs = new TaskCompletionSource<bool>();
                _frameAdapter.GoForward();

                await _tcs.Task;
            }
            finally
            {
                _tcs = null;
            }
        }

        private async Task GoBackWithFrame()
        {
            if (_tcs != null)
                throw new InvalidOperationException(StringResources.PendingNavigation);

            try
            {
                if (!await CanCloseAsync())
                    throw new TaskCanceledException();

                _tcs = new TaskCompletionSource<bool>();
                _frameAdapter.GoBack();

                await _tcs.Task;
            }
            finally
            {
                _tcs = null;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (_frameAdapter.HasError)
            {
                if (_tcs != null) _tcs.TrySetException(_frameAdapter.Error);
                return;
            }

            var prepareAsync = args.Parameter as Func<object, Task>;
            if (prepareAsync != null)
                prepareAsync(ActiveViewModel)
                    .ContinueWith(task =>
                                      {
                                          if (task.IsFaulted)
                                              _tcs.TrySetException(task.Exception);
                                          else if (task.IsCanceled)
                                              _tcs.TrySetCanceled();
                                          else
                                              _tcs.TrySetResult(true);
                                      });
            else if (_tcs != null)
                _tcs.TrySetResult(true);
        }

        private void OnNavigationStopped(object sender, NavigationEventArgs args)
        {
            if (_tcs != null)
                _tcs.TrySetCanceled();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            if (_tcs != null)
            {
                args.Handled = true;
                _tcs.TrySetException(args.Exception);
            }
        }

        /// <summary>
        ///   Determines if the target ViewModel type is authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <returns> Return true if the target type is authorized. </returns>
        private Task<bool> AuthorizeTargetAsync(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");

            if (_configuration.TargetGuard != null)
                return _configuration.TargetGuard(viewModelType);

            return TaskFns.FromResult(true);
        }
    }

    internal class FrameAdapterFix : FrameAdapter
    {
        private Exception _error;

        public FrameAdapterFix(Frame frame, bool treatViewAsLoaded = false) : base(frame, treatViewAsLoaded)
        {
        }

        public bool HasError
        {
            get { return _error != null; }
        }

        public Exception Error
        {
            get { return _error; }
        }

        protected override void OnNavigating(object sender, NavigatingCancelEventArgs args)
        {
            _error = null;
            base.OnNavigating(sender, args);
        }

        protected override void OnNavigated(object sender, NavigationEventArgs args)
        {
            // The Frame control doesn't properly pass the actual exception through to NavigationFailed. 
            // Let's capture it here and deal with it in the Navigator.
            try
            {
                base.OnNavigated(sender, args);
            }
            catch (Exception ex)
            {
                _error = ex;
            }
        }
    }

    internal partial class NavigatorConfiguration
    {
        public Func<Type, Task<bool>> TargetGuard { get; private set; }

        #region INavigatorConfigurator Members

        public INavigatorConfigurator WithTargetGuard(Func<Type, Task<bool>> guard)
        {
            TargetGuard = guard;
            return this;
        }

        #endregion
    }
}