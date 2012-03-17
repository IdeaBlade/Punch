﻿// ====================================================================================================================
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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    internal class CompositionHelper
    {
        private CompositionContainer _container;
        private ComposablePartCatalog _catalog;

        /// <summary>Returns the current catalog in use.</summary>
        /// <returns>Unless a custom catalog is provided through <see cref="Configure"/>, this property returns <see cref="AggregateCatalog"/></returns>
        public ComposablePartCatalog Catalog
        {
            get { return _catalog ?? AggregateCatalog; }
        }

        /// <summary>
        /// Returns the AggregateCatalog in use by DevForce.
        /// </summary>
        public AggregateCatalog AggregateCatalog
        {
            get { return CompositionHost.Instance.Catalog; }
        }

        /// <summary>Returns the CompositionContainer in use.</summary>
        public CompositionContainer Container
        {
            get { return _container ?? (_container = new CompositionContainer(Catalog)); }
        }

        /// <summary>Configures the CompositionHost.</summary>
        /// <param name="catalog">The custom catalog to be used by Cocktail to get access to MEF exports.</param>
        public void Configure(ComposablePartCatalog catalog)
        {
            Clear();
            _catalog = catalog;
        }

        /// <summary>Executes composition on the container, including the changes in the specified <see cref="CompositionBatch"/>.</summary>
        /// <param name="compositionBatch">
        /// 	Changes to the <see cref="CompositionContainer"/> to include during the composition.
        /// </param>
        public void Compose(CompositionBatch compositionBatch)
        {
            if (compositionBatch == null)
                throw new ArgumentNullException("compositionBatch");

            Container.Compose(compositionBatch);
        }

        /// <summary>
        /// Resets the CompositionContainer to it's initial state.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="Clear"/>, <see cref="Configure"/> should be called to configure the new CompositionContainer.
        /// </remarks>
        public void Clear()
        {
            if (_container != null)
                _container.Dispose();
            _container = null;
            _catalog = null;

            Cleared(null, EventArgs.Empty);
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            List<Export> exports = GetExportsCore(typeof (T), null, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  typeof (T).FullName));

            return exports.Select(e => e.Value).Cast<T>().First();
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            IEnumerable<Export> exports = GetExportsCore(typeof (T), null, requiredCreationPolicy);
            return exports.Select(e => e.Value).Cast<T>();
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specified, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public object GetInstance(Type serviceType, string key,
                                         CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            List<Export> exports = GetExportsCore(serviceType, key, requiredCreationPolicy).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  serviceType != null ? serviceType.ToString() : key));

            return exports.First().Value;
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public IEnumerable<object> GetInstances(Type serviceType,
                                                       CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            IEnumerable<Export> exports = GetExportsCore(serviceType, null, requiredCreationPolicy);
            return exports.Select(e => e.Value);
        }

        /// <summary>Satisfies all the imports on the provided instance.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Fired after <see cref="Clear"/> has been called to clear the current CompositionContainer.
        /// </summary>
        public event EventHandler<EventArgs> Cleared = delegate { };

        internal IEnumerable<Export> GetExportsCore(Type serviceType, string key, CreationPolicy policy)
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

            return Container.GetExports(importDef);
        }

        internal bool ExportExists<T>()
        {
            return Container.GetExports<T>().Any();
        }
    }
}