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
    public partial interface INavigationService
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
        Task<bool> GoForward();

        /// <summary>
        ///   Navigates to the most recent item in back navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        Task<bool> GoBack();
    }

    public partial class NavigationService
    {
        private readonly IConductActiveItem _conductor;
        private readonly Frame _frame;
        private readonly FrameAdapter _frameAdapter;
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        ///   Initializes a new NavigationService for ViewModel-based navigation.
        /// </summary>
        /// <param name="conductor"> The underlying screen conductor used to activate navigation targets. </param>
        public NavigationService(IConductActiveItem conductor)
        {
            if (conductor == null) throw new ArgumentNullException("conductor");

            _conductor = conductor;
        }

        /// <summary>
        ///   Initializes a new NavigationService for view-based navigation.
        /// </summary>
        /// <param name="frame"> The content control that supports navigation. </param>
        /// <param name="treatViewAsLoaded"> Treats the view as loaded if set to true. </param>
        public NavigationService(Frame frame, bool treatViewAsLoaded = false)
        {
            if (frame == null) throw new ArgumentNullException("frame");

            _frame = frame;
            _frameAdapter = new FrameAdapter(_frame, treatViewAsLoaded);
            _frameAdapter.Navigated += OnNavigated;
            _frameAdapter.NavigationFailed += OnNavigationFailed;
            _frameAdapter.NavigationStopped += OnNavigationStopped;
        }

        #region INavigationService Members

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
        public Task<bool> GoForward()
        {
            if (_frameAdapter == null)
                throw new NotSupportedException(
                    "The current NavigationService doesn't manage its own navigation history");

            return GoForwardWithFrame()
                .ContinueWith(task =>
                                  {
                                      _tcs = null;
                                      return task.Result;
                                  });
        }

        /// <summary>
        ///   Navigates to the most recent item in back navigation history, if a NavigationService manages its own navigation history.
        /// </summary>
        public Task<bool> GoBack()
        {
            if (_frameAdapter == null)
                throw new NotSupportedException(
                    "The current NavigationService doesn't manage its own navigation history");

            return GoBackWithFrame()
                .ContinueWith(task =>
                                  {
                                      _tcs = null;
                                      return task.Result;
                                  });
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
        public Task<bool> NavigateToAsync(Type viewModelType, Func<object, Task> prepare)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (prepare == null) throw new ArgumentNullException("prepare");

            if (_conductor != null)
                return NavigateWithConductor(viewModelType, prepare);

            return NavigateWithFrame(viewModelType, prepare)
                .ContinueWith(task =>
                                  {
                                      _tcs = null;
                                      return task.Result;
                                  });
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

        private async Task<bool> NavigateWithConductor(Type viewModelType, Func<object, Task> prepare)
        {
            if (!await GuardAsync(viewModelType))
                return false;

            var target = Composition.GetInstance(viewModelType, null);
            await prepare(target);

            if (!ReferenceEquals(ActiveViewModel, target))
                _conductor.ActivateItem(target);

            return true;
        }

        private async Task<bool> NavigateWithFrame(Type viewModelType, Func<object, Task> prepare)
        {
            if (_tcs != null)
                throw new InvalidOperationException("Another navigation is pending.");

            var viewType = ViewLocator.LocateTypeForModelType(viewModelType, null, null);
            if (viewType == null)
                throw new Exception(string.Format("No view was found for {0}. See the log for searched views.",
                                                  viewModelType.FullName));

            await GuardAsync(viewModelType);

            _tcs = new TaskCompletionSource<bool>();
            _frameAdapter.Navigate(viewType, prepare);

            return await _tcs.Task;
        }

        private async Task<bool> GoForwardWithFrame()
        {
            if (_tcs != null)
                throw new InvalidOperationException("Another navigation is pending.");

            if (!await CanCloseAsync())
                return false;

            _tcs = new TaskCompletionSource<bool>();
            _frameAdapter.GoForward();

            return await _tcs.Task;
        }

        private async Task<bool> GoBackWithFrame()
        {
            if (_tcs != null)
                throw new InvalidOperationException("Another navigation is pending.");

            if (!await CanCloseAsync())
                return false;

            _tcs = new TaskCompletionSource<bool>();
            _frameAdapter.GoBack();

            return await _tcs.Task;
        }

        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            var prepareAsync = args.Parameter as Func<object, Task>;
            if (prepareAsync != null)
                prepareAsync(ActiveViewModel).ContinueWith(task => _tcs.TrySetResult(true));
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
                _tcs.TrySetException(args.Exception);
        }
    }
}