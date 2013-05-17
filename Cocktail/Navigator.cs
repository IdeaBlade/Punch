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
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>
    ///   Interface used to configure a NavigationService.
    /// </summary>
    public partial interface INavigatorConfigurator : IHideObjectMembers
    {
        /// <summary>
        ///   Configures the close guard for the ActiveViewModel.
        /// </summary>
        /// <param name="guard"> The close guard. </param>
        /// <remarks>
        ///   if no guard is configured, <see cref="IGuardClose.CanClose" /> is automatically being called.
        /// </remarks>
        INavigatorConfigurator WithActiveItemGuard(Func<object, Task<bool>> guard);
    }

    /// <summary>
    ///   A service that performs UI navigation logic.
    /// </summary>
    public partial interface INavigator : IHideObjectMembers
    {
        /// <summary>
        ///   Returns the current active ViewModel or null.
        /// </summary>
        object ActiveViewModel { get; }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync(Type viewModelType, Func<object, Task> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync(Type viewModelType, Action<object> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync(Type viewModelType, object parameter = null);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync<T>(Func<T, Task> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync<T>(Action<T> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task NavigateToAsync<T>(object parameter = null);
    }

    /// <summary>
    ///   A configurable service that performs UI navigation logic.
    /// </summary>
    public partial class Navigator : INavigator
    {
        private readonly NavigatorConfiguration _configuration = new NavigatorConfiguration();

        #region INavigator Members

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync(Type viewModelType, Action<object> prepare)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (prepare == null) throw new ArgumentNullException("prepare");

            return NavigateToAsync(viewModelType, viewModel =>
                                                      {
                                                          prepare(viewModel);
                                                          return TaskFns.FromResult(true);
                                                      });
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync<T>(Func<T, Task> prepare)
        {
            if (prepare == null) throw new ArgumentNullException("prepare");

            return NavigateToAsync(typeof(T), viewModel => prepare((T) viewModel));
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync<T>(Action<T> prepare)
        {
            if (prepare == null) throw new ArgumentNullException("prepare");

            return NavigateToAsync<T>(viewModel =>
                                          {
                                              prepare(viewModel);
                                              return TaskFns.FromResult(true);
                                          });
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <param name="parameter">An optional parameter to be sent to the target view model. See <see cref="INavigationTarget"/></param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task NavigateToAsync<T>(object parameter = null)
        {
            return NavigateToAsync(typeof(T), parameter);
        }

        #endregion

        /// <summary>
        /// A static helper that can be used as prepare action in order to inject a parameter into the target view model
        /// </summary>
        /// <param name="target">The target view model</param>
        /// <param name="parameter">The parameter value to be injected.</param>
        /// <param name="propertyName">The name of the property to inject to.</param>
        public static void TryInjectParameter(object target, object parameter, string propertyName = "Parameter")
        {
            var viewModelType = target.GetType();
            var property = viewModelType.GetPropertyCaseInsensitive(propertyName);

            if (property == null)
                return;

#if SILVERLIGHT
            property.SetValue(target, MessageBinder.CoerceValue(property.PropertyType, parameter, null), null);
#else
            property.SetValue(target, MessageBinder.CoerceValue(property.PropertyType, parameter, null));
#endif
        }

        /// <summary>
        ///   Configures the current NavigationService.
        /// </summary>
        /// <param name="configure"> A delegate action to perform the configuration. </param>
        public Navigator Configure(Action<INavigatorConfigurator> configure)
        {
            configure(_configuration);
            return this;
        }

        /// <summary>
        ///   Determines if the active ViewModel can be closed.
        /// </summary>
        /// <returns> Returns true if the active ViewModel can be closed. </returns>
        private Task<bool> CanCloseAsync()
        {
            if (ActiveViewModel == null)
                return TaskFns.FromResult(true);

            if (_configuration.ActiveItemGuard != null)
                return _configuration.ActiveItemGuard(ActiveViewModel);

            var closeGuard = ActiveViewModel as IGuardClose;
            if (closeGuard != null)
                return TaskFns.FromCallbackAction<bool>(closeGuard.CanClose);

            return TaskFns.FromResult(true);
        }
    }

    internal partial class NavigatorConfiguration : INavigatorConfigurator
    {
        public Func<object, Task<bool>> ActiveItemGuard { get; private set; }

        #region INavigatorConfigurator Members

        public INavigatorConfigurator WithActiveItemGuard(Func<object, Task<bool>> guard)
        {
            ActiveItemGuard = guard;
            return this;
        }

        #endregion
    }
}