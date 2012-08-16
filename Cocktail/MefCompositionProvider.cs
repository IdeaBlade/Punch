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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Cocktail
{
    /// <summary>
    /// An implementation of <see cref="ICompositionProvider"/> which uses MEF as the underlying IoC implementation.
    /// </summary>
    internal partial class MefCompositionProvider : ICompositionProvider
    {
        /// <summary>
        /// Returns true if the provided type has been previously registered.
        /// </summary>
        public bool IsTypeRegistered<T>()
        {
            return Container.GetExports<T>().Any();
        }

        /// <summary>
        ///   Returns an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        public T GetInstance<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstance<T>(instanceType).Value;
        }

        /// <summary>
        ///   Returns all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        public IEnumerable<T> GetInstances<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstances<T>().Select(x => x.Value);
        }

        /// <summary>
        ///   Returns an instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        public object GetInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstance(serviceType, contractName, instanceType).Value;
        }

        /// <summary>
        ///   Returns all instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> Type of the requested instances. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        public IEnumerable<object> GetInstances(Type serviceType, InstanceType instanceType = InstanceType.NotSpecified)
        {
            return GetLazyInstances(serviceType, instanceType).Select(x => x.Value);
        }

        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        public Lazy<T> GetLazyInstance<T>(InstanceType instanceType = InstanceType.NotSpecified)
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
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        public IEnumerable<Lazy<T>> GetLazyInstances<T>(InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(typeof(T), null, ConvertToCreationPolicy(instanceType));
            return ConvertToLazy<T>(exports);
        }

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        /// <returns> The requested instance. </returns>
        public Lazy<object> GetLazyInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified)
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
        /// <param name="serviceType"> Type of the requested instances. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        /// <returns> The requested instances. </returns>
        public IEnumerable<Lazy<object>> GetLazyInstances(Type serviceType, InstanceType instanceType = InstanceType.NotSpecified)
        {
            var exports = GetExportsCore(serviceType, null, ConvertToCreationPolicy(instanceType));
            return ConvertToLazy<object>(exports);
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
    }
}
