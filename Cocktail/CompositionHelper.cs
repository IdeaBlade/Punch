//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.EntityModel;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    /// <summary>
    /// Sets up a composition container and provides means to interact with the container.
    /// </summary>
    public static class CompositionHelper
    {
        private static AggregateCatalog _catalog;
        private static CompositionContainer _container;
        private static bool _isConfigured;

        #region Public Properties

        /// <summary>
        /// Returns true if the CompositonHost has been configured.
        /// </summary>
        public static bool IsConfigured
        {
            get { return _isConfigured && !IsInDesignMode(); }
        }

        /// <summary>Returns the catalog in use.</summary>
        public static AggregateCatalog Catalog
        {
            get { return _catalog ?? (_catalog = CompositionHost.Instance.Catalog); }
        }

        /// <summary>Returns the CompositionContainer in use.</summary>
        public static CompositionContainer Container { get { return _container; } }

        #endregion

        /// <summary>Configures the CompositionHost.</summary>
        /// <param name="compositionBatch">
        ///     Optional changes to the <span><see cref="CompositionContainer"/></span> to include during the composition.
        /// </param>
        public static void Configure(CompositionBatch compositionBatch = null)
        {
            if (IsInDesignMode()) return;

            if (IsConfigured) return;

            _container = GetContainer();
            _isConfigured = true;

            CompositionBatch batch = compositionBatch ?? new CompositionBatch();

            if (!ExportExists<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            if (!ExportExists<IAuthenticationProvider>())
                batch.AddExportedValue<IAuthenticationProvider>(new AuthenticationManagerProvider());

            Compose(batch);
        }

        /// <summary>Executes composition on the container, including the changes in the specified <see cref="CompositionBatch"/>.</summary>
        /// <param name="compositionBatch">
        /// 	Changes to the <see cref="CompositionContainer"/> to include during the composition.
        /// </param>
        public static void Compose(CompositionBatch compositionBatch)
        {
            CheckIfConfigured();

            if (compositionBatch == null)
                throw new ArgumentNullException("compositionBatch");

            _container.Compose(compositionBatch);
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(_container, typeof(T), null, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  typeof(T).FullName));

            return exports.Select(e => e.Value).Cast<T>().First();
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return GetInstances<T>(_container, requiredCreationPolicy);
        }

        internal static IEnumerable<T> GetInstances<T>(CompositionContainer container, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(container, typeof(T), null, requiredCreationPolicy);
            return exports.Select(e => e.Value).Cast<T>();
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specifed, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static object GetInstance(Type serviceType, string key, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            var exports = GetExportsCore(_container, serviceType, key, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  serviceType != null ? serviceType.ToString() : key));

            return exports.First().Value;
        }

        /// <summary>Obsolete. Will be removed from the framework in a future build.</summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Use GetInstance(,,) with requiredCreationPolicy=CreationPolicy.NonShared instead.")]
        public static object GetNewInstance(Type serviceType, string key)
        {
            return GetInstance(serviceType, key, CreationPolicy.NonShared);
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<object> GetInstances(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            IEnumerable<Export> exports = GetExportsCore(_container, serviceType, null, requiredCreationPolicy);

            return exports.Select(e => e.Value);
        }

        /// <summary>Obsolete. Will be removed from the framework in a future build.</summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [Obsolete("Use GetInstances(,) with requiredCreationPolicy=CreationPolicy.NonShared instead.")]
        public static IEnumerable<object> GetNewInstances(Type serviceType)
        {
            return GetInstances(serviceType, CreationPolicy.NonShared);
        }

        /// <summary>Returns an instance of the custom implementation for the provided type. If no custom implementation is found, an instance of the default
        /// implementation is returned.</summary>
        /// <param name="serviceType">The type for which an instance is being requested.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static object GetCustomInstanceOrDefault(Type serviceType, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return GetCustomInstanceOrDefault(serviceType, _container, requiredCreationPolicy);
        }

        internal static object GetCustomInstanceOrDefault(Type serviceType, CompositionContainer container, CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            CheckIfConfigured();

            if (IsInDesignMode()) return GetInstance(serviceType, null, requiredCreationPolicy);

            //  Find all exports
            var exports = GetExportsCore(container, serviceType, null, requiredCreationPolicy, true).ToList();

            // Filter exports for custom implementations
            var customExports = exports.Where(e => !e.Metadata.ContainsKey("IsDefault")).ToList();
            if (customExports.Any()) return customExports.First().Value;

            // Filter exports for default implementations
            var defaultExports = exports.Where(e => e.Metadata.ContainsKey("IsDefault")).ToList();
            if (defaultExports.Any()) return defaultExports.First().Value;

            throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract, serviceType));
        }

        internal static IEnumerable<Export> GetExportsCore(CompositionContainer container, Type serviceType, string key, CreationPolicy policy, bool includeDefaults = false)
        {
            string contractName = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            string requiredTypeIdentity = serviceType != null
                                              ? AttributedModelServices.GetTypeIdentity(serviceType)
                                              : null;
            var importDef = new ContractBasedImportDefinition(
                contractName,
                requiredTypeIdentity,
                Enumerable.Empty<KeyValuePair<string, Type>>(),
                ImportCardinality.ZeroOrMore,
                false,
                true,
                policy);

            return container.GetExports(importDef).Where(e => includeDefaults || !e.Metadata.ContainsKey("IsDefault"));
        }

        /// <summary>Satisfies all the imports on the provided instance.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public static void BuildUp(object instance)
        {
            BuildUp(instance, _container);
        }

        internal static void BuildUp(object instance, CompositionContainer container)
        {
            CheckIfConfigured();

            container.SatisfyImportsOnce(instance);
        }

        internal static bool ExportExists<T>()
        {
            var container = _container ?? GetContainer();
            return container.GetExports<T>().Any();
        }

        internal static bool IsRecomposing { get; set; }

        #region Private Methods

        private static void CheckIfConfigured()
        {
            if (!IsConfigured)
                throw new InvalidOperationException(StringResources.CompositionHelperIsNotConfigured);
        }

        private static CompositionContainer GetContainer()
        {
            return CompositionHost.Instance.Container;
        }

        #endregion

        #region DesignTime Functionality

        private static readonly Func<bool> IsInDesignModeDefault = () => Execute.InDesignMode;

        /// <summary>Function to determine if in DesignMode. Can be replaced for testing.</summary>
        /// <value>A delegate returning true if in design mode.</value>
        public static Func<bool> IsInDesignMode = IsInDesignModeDefault;

        /// <summary>
        /// Restore <see cref="IsInDesignMode"/> to default method. For testing.
        /// </summary>
        public static void ResetIsInDesignModeToDefault()
        {
            IsInDesignMode = IsInDesignModeDefault;
        }

        #endregion
    }
}