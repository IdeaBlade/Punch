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
    /// Interface used to configure a NavigationService.
    /// </summary>
    /// <typeparam name="T">The navigation target type.</typeparam>
    public interface INavigationServiceConfigurator<T> : IHideObjectMembers where T : class
    {
        /// <summary>
        /// Configures the activator action for the current NavigationService. The activator is responsible for activating the target in the current conductor.
        /// </summary>
        /// <param name="activator">The activator action to be called upon to activate a new navigation target.</param>
        /// <remarks>If no activator is configured <see cref="IConductor.ActivateItem"/> is called automatically.</remarks>
        INavigationServiceConfigurator<T> WithActivator(Action<NavigationProcessor<T>> activator);

        /// <summary>
        /// Configures the strategy to determine whether navigating away from the current target is permissible.
        /// </summary>
        /// <param name="strategy">The new navigation strategy.</param>
        /// <remarks>If no navigation strategy is configured, <see cref="IGuardClose.CanClose"/> of the current target is used to determine if navigating away is currently permissible.</remarks>
        INavigationServiceConfigurator<T> WithNavigateAwayStrategy(Action<NavigationProcessor<T>, Action<bool>> strategy);

        /// <summary>
        /// Configures the strategy to determine whether navigating to the new target is permissible.
        /// </summary>
        /// <param name="strategy">The new navigation strategy.</param>
        INavigationServiceConfigurator<T> WithNavigateToStrategy(Action<NavigationProcessor<T>, Action<bool>> strategy);
    }

    /// <summary>
    /// A configurable service to handle navigation.
    /// </summary>
    /// <typeparam name="T">The type of the ViewModel.</typeparam>
    public class NavigationService<T> where T : class
    {
        private readonly IConductActiveItem _conductor;
        private readonly NavigationServiceConfiguration<T> _configuration;

        /// <summary>
        /// Initializes a new NavigationService.
        /// </summary>
        /// <param name="conductor">The underlying screen conductor used to activate navigation targets.</param>
        public NavigationService(IConductActiveItem conductor)
        {
            _conductor = conductor;
            _configuration = new NavigationServiceConfiguration<T>();
        }

        /// <summary>
        /// Configures the current NavigationService.
        /// </summary>
        /// <param name="configure">A delegate action to perform the configuration.</param>
        public NavigationService<T> Configure(Action<INavigationServiceConfigurator<T>> configure)
        {
            configure(_configuration);
            return this;
        }

        /// <summary>
        /// Asynchronously navigates to the specified target view model.
        /// </summary>
        /// <param name="target">A function to determine the navigation target.</param>
        /// <param name="prepareTarget">An optional function to prepare the navigation target before it gets activated.</param>
        /// <param name="prepareTargetAsync">An optional function to asynchronously prepare the navigation target before it gets activated.</param>
        /// <returns>The asynchronous navigation <see cref="Task"/>.</returns>
        public Task NavigateToAsync(Func<T> target, Action<T> prepareTarget = null, 
                                    Func<T, Task> prepareTargetAsync = null)
        {
            var processor = new NavigationProcessor<T>(_conductor, target);
            if (_configuration.Activator != null)
                processor.Activate = _configuration.Activator;
            if (_configuration.NavigateAwayStrategy != null)
                processor.CanNavigateAway = _configuration.NavigateAwayStrategy;
            if (_configuration.NavigateToStrategy != null)
                processor.CanNavigateTo = _configuration.NavigateToStrategy;

            if (prepareTarget != null)
                processor.Prepare = nav => prepareTarget(nav.Target);
            if (prepareTargetAsync != null)
                processor.PrepareAsync = nav => prepareTargetAsync(nav.Target);

            return processor.NavigateAsync();
        }
    }

    internal class NavigationServiceConfiguration<T> : INavigationServiceConfigurator<T>
        where T : class
    {
        public Action<NavigationProcessor<T>> Activator { get; private set; }

        public Action<NavigationProcessor<T>, Action<bool>> NavigateAwayStrategy { get; private set; }

        public Action<NavigationProcessor<T>, Action<bool>> NavigateToStrategy { get; private set; }

        #region INavigationServiceConfigurator<T> Members

        public INavigationServiceConfigurator<T> WithActivator(Action<NavigationProcessor<T>> activator)
        {
            Activator = activator;
            return this;
        }

        public INavigationServiceConfigurator<T> WithNavigateAwayStrategy(
            Action<NavigationProcessor<T>, Action<bool>> strategy)
        {
            NavigateAwayStrategy = strategy;
            return this;
        }

        public INavigationServiceConfigurator<T> WithNavigateToStrategy(Action<NavigationProcessor<T>, Action<bool>> strategy)
        {
            NavigateToStrategy = strategy;
            return this;
        }

        #endregion
    }
}