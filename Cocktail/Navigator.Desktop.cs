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
        INavigatorConfigurator WithTargetGuard(Func<Type, string, Task<bool>> guard);
    }

    public partial interface INavigator
    {
        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        Task NavigateToAsync(Type viewModelType, string contractName, Func<object, Task> prepare);

        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        Task NavigateToAsync(Type viewModelType, string contractName, Action<object> prepare);

        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        Task NavigateToAsync(Type viewModelType, string contractName, object parameter = null);
    }

    public partial class Navigator
    {
        private readonly IConductActiveItem _conductor;

        /// <summary>
        ///   Initializes a new NavigationService.
        /// </summary>
        /// <param name="conductor"> The underlying screen conductor used to activate navigation targets. </param>
        public Navigator(IConductActiveItem conductor)
        {
            _conductor = conductor;
        }

        #region INavigator Members

        /// <summary>
        ///   Returns the current active ViewModel or null.
        /// </summary>
        public object ActiveViewModel
        {
            get { return _conductor.ActiveItem; }
        }

        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        public async Task NavigateToAsync(Type viewModelType, string contractName, object parameter = null)
        {
            if (viewModelType == null && string.IsNullOrEmpty(contractName))
                throw new ArgumentNullException();

            if (!await CanCloseAsync())
                throw new TaskCanceledException();

            await (ActiveViewModel as INavigationTarget).OnNavigatingFromAsync();

            if (!await AuthorizeTargetAsync(viewModelType, contractName))
                throw new TaskCanceledException();

            var target = Composition.GetInstance(viewModelType, contractName);
            var prepareAction = parameter as Func<object, Task>;
            if (prepareAction != null)
                await prepareAction(target);

            await (target as INavigationTarget).OnNavigatedToAsync(prepareAction == null ? parameter : null);

            if (!ReferenceEquals(ActiveViewModel, target))
                _conductor.ActivateItem(target);

            await (ActiveViewModel as INavigationTarget).OnNavigatedFromAsync(prepareAction == null ? parameter : null);
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

            return NavigateToAsync(viewModelType, null, prepare);
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync(Type viewModelType, object parameter = null)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");

            return NavigateToAsync(viewModelType, null, parameter);
        }

        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        public Task NavigateToAsync(Type viewModelType, string contractName, Func<object, Task> prepare)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (prepare == null) throw new ArgumentNullException("prepare");

            return NavigateToAsync(viewModelType, contractName, (object) prepare);
        }

        /// <summary>
        ///   Asynchronously navigates to a ViewModel instance that matches the specified name and type. 
        ///   The navigation will be cancelled if the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The type to match. </param>
        /// <param name="contractName"> The name to match. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        /// <remarks>
        ///   Not available in Windows 8 Store apps.
        /// </remarks>
        public Task NavigateToAsync(Type viewModelType, string contractName, Action<object> prepare)
        {
            if (viewModelType == null && string.IsNullOrEmpty(contractName))
                throw new ArgumentNullException();
            if (prepare == null)
                throw new ArgumentNullException("prepare");

            return NavigateToAsync(viewModelType, contractName, viewModel =>
                                                                    {
                                                                        prepare(viewModel);
                                                                        return TaskFns.FromResult(true);
                                                                    });
        }

        #endregion

        /// <summary>
        ///   Determines if the target ViewModel type is authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="contractName">The target ViewModel contract name.</param>
        /// <returns> Return true if the target type is authorized. </returns>
        private Task<bool> AuthorizeTargetAsync(Type viewModelType, string contractName)
        {
            if (viewModelType == null && string.IsNullOrEmpty(contractName))
                throw new ArgumentNullException();

            if (_configuration.TargetGuard != null)
                return _configuration.TargetGuard(viewModelType, contractName);

            return TaskFns.FromResult(true);
        }
    }

    internal partial class NavigatorConfiguration
    {
        public Func<Type, string, Task<bool>> TargetGuard { get; private set; }

        #region INavigatorConfigurator Members

        public INavigatorConfigurator WithTargetGuard(Func<Type, string, Task<bool>> guard)
        {
            TargetGuard = guard;
            return this;
        }

        #endregion
    }
}