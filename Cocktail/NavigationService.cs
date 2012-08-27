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
    public interface INavigationServiceConfigurator : IHideObjectMembers
    {
        /// <summary>
        ///   Configures the close guard for the ActiveViewModel.
        /// </summary>
        /// <param name="guard"> The close guard. </param>
        /// <remarks>
        ///   if no guard is configured, <see cref="IGuardClose.CanClose" /> is automatically being called.
        /// </remarks>
        INavigationServiceConfigurator WithActiveItemGuard(Func<object, Task<bool>> guard);

        /// <summary>
        ///   Configures the guard for the target type.
        /// </summary>
        /// <param name="guard"> The target guard. </param>
        /// <remarks>
        ///   The target guard controls whether the target type is an authorized navigation target.
        /// </remarks>
        INavigationServiceConfigurator WithTargetGuard(Func<Type, Task<bool>> guard);
    }

    /// <summary>
    ///   A service that implements UI navigation logic.
    /// </summary>
    public partial interface INavigationService
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
        Task<bool> NavigateToAsync(Type viewModelType, Func<object, Task> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task<bool> NavigateToAsync(Type viewModelType, Action<object> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task<bool> NavigateToAsync(Type viewModelType);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task<bool> NavigateToAsync<T>(Func<T, Task> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task<bool> NavigateToAsync<T>(Action<T> prepare);

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        Task<bool> NavigateToAsync<T>();
    }

    /// <summary>
    ///   A configurable service that implements UI navigation logic.
    /// </summary>
    public partial class NavigationService : INavigationService
    {
        private readonly NavigationServiceConfiguration _configuration = new NavigationServiceConfiguration();

        #region INavigationService Members

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task<bool> NavigateToAsync(Type viewModelType, Action<object> prepare)
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
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task<bool> NavigateToAsync(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");

            return NavigateToAsync(viewModelType, viewModel => TaskFns.FromResult(true));
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <typeparam name="T"> The target ViewModel type. </typeparam>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task<bool> NavigateToAsync<T>(Func<T, Task> prepare)
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
        public Task<bool> NavigateToAsync<T>(Action<T> prepare)
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
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public Task<bool> NavigateToAsync<T>()
        {
            return NavigateToAsync<T>(viewModel => TaskFns.FromResult(true));
        }

        #endregion

        /// <summary>
        ///   Configures the current NavigationService.
        /// </summary>
        /// <param name="configure"> A delegate action to perform the configuration. </param>
        public NavigationService Configure(Action<INavigationServiceConfigurator> configure)
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

    internal class NavigationServiceConfiguration : INavigationServiceConfigurator
    {
        public Func<object, Task<bool>> ActiveItemGuard { get; private set; }

        public Func<Type, Task<bool>> TargetGuard { get; private set; }

        #region INavigationServiceConfigurator Members

        public INavigationServiceConfigurator WithActiveItemGuard(Func<object, Task<bool>> guard)
        {
            ActiveItemGuard = guard;
            return this;
        }

        public INavigationServiceConfigurator WithTargetGuard(Func<Type, Task<bool>> guard)
        {
            TargetGuard = guard;
            return this;
        }

        #endregion
    }
}