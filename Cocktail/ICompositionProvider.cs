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

namespace Cocktail
{
    /// <summary>
    /// Specifies if an instance returned by <see cref="ICompositionProvider"/> should
    /// be shared, not shared or left to the provider to decide.
    /// </summary>
    public enum InstanceType { Shared, NonShared, NotSpecified };

    /// <summary>
    /// A service providing implementation independent IoC functionality.
    /// </summary>
    public partial interface ICompositionProvider
    {
        /// <summary>
        /// Returns true if the provided type has been previously registered.
        /// </summary>
        bool IsTypeRegistered<T>();

        /// <summary>
        ///   Returns an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        T GetInstance<T>(InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        IEnumerable<T> GetInstances<T>(InstanceType instanceType = InstanceType.NotSpecified);
    
        /// <summary>
        ///   Returns an instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        object GetInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns all instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> Type of the requested instances. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        IEnumerable<object> GetInstances(Type serviceType, InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        Lazy<T> GetLazyInstance<T>(InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns all lazy instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        IEnumerable<Lazy<T>> GetLazyInstances<T>(InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instance should be a shared or not shared. </param>
        Lazy<object> GetLazyInstance(Type serviceType, string contractName, InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>
        ///   Returns all lazy instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> Type of the requested instances. </param>
        /// <param name="instanceType"> Optionally specify whether the returned instances should be a shared or not shared. </param>
        IEnumerable<Lazy<object>> GetLazyInstances(Type serviceType, InstanceType instanceType = InstanceType.NotSpecified);

        /// <summary>Manually performs property dependency injection on the provided instance.</summary>
        /// <param name="instance">The instance needing property injection.</param>
        void BuildUp(object instance);
    }
}
