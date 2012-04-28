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
        INavigationServiceConfigurator<T> WithActivator(Action<NavigateResult<T>> activator);

        /// <summary>
        /// Configures the strategy to determine whether navigating away from the current target is permissible.
        /// </summary>
        /// <param name="strategy">The new navigation strategy.</param>
        /// <remarks>If no navigation strategy is configured, <see cref="IGuardClose.CanClose"/> of the current target is used to determine if navigating away is currently permissible.</remarks>
        INavigationServiceConfigurator<T> WithNavigateAwayStrategy(Action<NavigateResult<T>, Action<bool>> strategy);

        /// <summary>
        /// Configures the strategy to determine whether navigating to the new target is permissible.
        /// </summary>
        /// <param name="strategy">The new navigation strategy.</param>
        INavigationServiceConfigurator<T> WithNavigateToStrategy(Action<NavigateResult<T>, Action<bool>> strategy);
    }

    /// <summary>
    /// A configurable service to handle navigation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        /// <returns>An object representing the asynchronous navigation operation.</returns>
        public NavigateResult<T> NavigateToAsync(Func<T> target, Action<T> prepareTarget = null,
                                                 Func<T, IResult> prepareTargetAsync = null)
        {
            var navigation = new NavigateResult<T>(_conductor, target);
            if (_configuration.Activator != null)
                navigation.Activate = _configuration.Activator;
            if (_configuration.NavigateAwayStrategy != null)
                navigation.CanNavigateAway = _configuration.NavigateAwayStrategy;
            if (_configuration.NavigateToStrategy != null)
                navigation.CanNavigateTo = _configuration.NavigateToStrategy;

            if (prepareTarget != null)
                navigation.Prepare = nav => prepareTarget(nav.Target);
            if (prepareTargetAsync != null)
                navigation.PrepareAsync = nav => prepareTargetAsync(nav.Target);

            navigation.Go();
            return navigation;
        }
    }

    internal class NavigationServiceConfiguration<T> : INavigationServiceConfigurator<T>
        where T : class
    {
        public Action<NavigateResult<T>> Activator { get; private set; }

        public Action<NavigateResult<T>, Action<bool>> NavigateAwayStrategy { get; private set; }

        public Action<NavigateResult<T>, Action<bool>> NavigateToStrategy { get; private set; }

        #region INavigationServiceConfigurator<T> Members

        public INavigationServiceConfigurator<T> WithActivator(Action<NavigateResult<T>> activator)
        {
            Activator = activator;
            return this;
        }

        public INavigationServiceConfigurator<T> WithNavigateAwayStrategy(
            Action<NavigateResult<T>, Action<bool>> strategy)
        {
            NavigateAwayStrategy = strategy;
            return this;
        }

        public INavigationServiceConfigurator<T> WithNavigateToStrategy(Action<NavigateResult<T>, Action<bool>> strategy)
        {
            NavigateToStrategy = strategy;
            return this;
        }

        #endregion
    }
}