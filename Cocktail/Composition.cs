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

using IdeaBlade.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocktail
{
    /// <summary>
    ///   Sets up a composition container and provides means to interact with the container.
    /// </summary>
    public static partial class Composition
    {
        private static ICompositionProvider _provider;

        static Composition()
        {
            EntityManager.EntityManagerCreated += OnEntityManagerCreated;
        }

        private static void OnEntityManagerCreated(object sender, EntityManagerCreatedEventArgs args)
        {
            if (!args.EntityManager.IsClient)
                return;

            var locator = new PartLocator<IAuthenticationService>(
                InstanceType.Shared, () => args.EntityManager.CompositionContext);
            if (locator.IsAvailable)
                args.EntityManager.AuthenticationContext = locator.GetPart().AuthenticationContext;
        }

        /// <summary>
        /// Sets the current <see cref="ICompositionProvider"/>.
        /// </summary>
        public static void SetProvider(ICompositionProvider compositionProvider)
        {
            if (compositionProvider == null)
                throw new ArgumentNullException(StringResources.CompositionProviderCannotBeNull);
            _provider = compositionProvider;
            ProviderChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// Returns true if the provided type has been previously registered.
        /// </summary>
        public static bool IsTypeRegistered<T>()
        {
            return Provider.IsTypeRegistered<T>();
        }

        /// <summary>
        ///   Returns an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static T GetInstance<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstance<T>(instanceType).Value;
        }

        /// <summary>
        ///   Returns all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static IEnumerable<T> GetInstances<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstances<T>(instanceType).Select(x => x.Value);
        }

        /// <summary>
        ///   Returns an instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static object GetInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstance(serviceType, contractName, instanceType).Value;
        }

        /// <summary>
        ///   Returns all instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static IEnumerable<object> GetInstances(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstances(serviceType, contractName, instanceType).Select(x => x.Value);
        }

        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static Lazy<T> GetLazyInstance<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return Provider.GetInstance<T>(instanceType);
        }

        /// <summary>
        ///   Returns all lazy instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static IEnumerable<Lazy<T>> GetLazyInstances<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return Provider.GetInstances<T>(instanceType);
        }

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static Lazy<object> GetLazyInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return Provider.GetInstance(serviceType, contractName, instanceType);
        }

        /// <summary>
        ///   Returns all lazy instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <remarks>
        ///    Not every <see cref=" ICompositionProvider"/> supports specifying an instanceType. 
        ///    If instanceType is not supported, a <see cref="NotSupportedException"/> is expected if instanceType is anything 
        ///    other than <see cref="InstanceType.NotSpecified"/>.
        /// </remarks>
        public static IEnumerable<Lazy<object>> GetLazyInstances(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return Provider.GetInstances(serviceType, contractName, instanceType);
        }

        /// <summary>Manually performs property dependency injection on the provided instance.</summary>
        /// <param name="instance">The instance needing property injection.</param>
        public static void BuildUp(object instance)
        {
            Provider.BuildUp(instance);
        }

        /// <summary>
        /// Event triggered after a new CompositionProvider was assigned through <see cref="SetProvider"/>.
        /// </summary>
        internal static event EventHandler<EventArgs> ProviderChanged = delegate { };

        internal static ICompositionProvider Provider
        {
            get
            {
                if (_provider == null)
                    throw new InvalidOperationException(StringResources.CompositionProviderNotConfigured);
                return _provider;
            }
        }
    }
}