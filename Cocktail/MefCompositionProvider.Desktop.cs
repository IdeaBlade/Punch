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

using Caliburn.Micro;
using IdeaBlade.Core.Composition;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    internal partial class MefCompositionProvider : ICompositionProvider
    {
        private CompositionContainer _container;
        private ComposablePartCatalog _catalog;

        /// <summary>
        ///   Returns the current catalog in use.
        /// </summary>
        /// <returns> Unless a custom catalog is provided through <see cref="Configure" />, this property returns <see cref="DefaultCatalog" /> </returns>
        public ComposablePartCatalog Catalog
        {
            get { return _catalog ?? DefaultCatalog; }
        }

        /// <summary>
        ///   Returns the default catalog in use by DevForce.
        /// </summary>
        public ComposablePartCatalog DefaultCatalog
        {
            get { return CompositionHost.Instance.Container.Catalog; }
        }

        /// <summary>
        ///   Returns the CompositionContainer in use.
        /// </summary>
        public CompositionContainer Container
        {
            get { return _container ?? (_container = new CompositionContainer(Catalog)); }
        }

        /// <summary>
        ///   Configures the CompositionHost.
        /// </summary>
        /// <param name="compositionBatch"> Optional changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        /// <param name="catalog"> The custom catalog to be used by Cocktail to get access to MEF exports. </param>
        public void Configure(CompositionBatch compositionBatch = null, ComposablePartCatalog catalog = null)
        {
            _catalog = catalog;

            var batch = compositionBatch ?? new CompositionBatch();
            if (!IsTypeRegistered<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            Compose(batch);
        }


        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        public Lazy<T> GetInstance<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(typeof(T), null, ConvertToCreationPolicy(instanceType)).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  typeof(T).FullName));

            return ConvertToLazy<T>(exports).First();
        }

        /// <summary>
        ///   Returns all lazy instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        public IEnumerable<Lazy<T>> GetInstances<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(typeof(T), null, ConvertToCreationPolicy(instanceType));
            return ConvertToLazy<T>(exports);
        }

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        public Lazy<object> GetInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(serviceType, contractName, ConvertToCreationPolicy(instanceType)).ToList();
            if (!exports.Any())
                throw new Exception(string.Format(StringResources.CouldNotLocateAnyInstancesOfContract,
                                                  serviceType != null ? serviceType.ToString() : contractName));

            return ConvertToLazy<object>(exports).First();
        }

        /// <summary>
        ///   Returns all lazy instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        public IEnumerable<Lazy<object>> GetInstances(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(serviceType, contractName, ConvertToCreationPolicy(instanceType));
            return ConvertToLazy<object>(exports);
        }

        /// <summary>Manually performs property dependency injection on the provided instance.</summary>
        /// <param name="instance">The instance needing property injection.</param>
        public void BuildUp(object instance)
        {
            // Skip if in design mode.
            if (Execute.InDesignMode)
                return;

            Container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        ///   Executes composition on the container, including the changes in the specified <see cref="CompositionBatch" /> .
        /// </summary>
        /// <param name="compositionBatch"> Changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        public void Compose(CompositionBatch compositionBatch)
        {
            if (compositionBatch == null)
                throw new ArgumentNullException("compositionBatch");

            Container.Compose(compositionBatch);
        }

        private IEnumerable<Export> GetExportsCore(Type serviceType, string key, CreationPolicy policy)
        {
            var contractName = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var requiredTypeIdentity = serviceType != null
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

        private CreationPolicy ConvertToCreationPolicy(InstanceType instanceType)
        {
            if (instanceType == InstanceType.Shared)
                return CreationPolicy.Shared;
            else if (instanceType == InstanceType.NonShared)
                return CreationPolicy.NonShared;

            return CreationPolicy.Any;
        }

        private IEnumerable<Lazy<T>> ConvertToLazy<T>(IEnumerable<Export> exports)
        {
            return exports.Select(e => new Lazy<T>(() => (T)e.Value));
        }

        /// <summary>
        ///   Fired when the composition container is modified after initialization.
        /// </summary>
        public event EventHandler<RecomposedEventArgs> Recomposed
        {
            add { CompositionHost.Recomposed += value; }
            remove { CompositionHost.Recomposed -= value; }
        }
    }
}
