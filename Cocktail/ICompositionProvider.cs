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

using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using System;
using System.Collections.Generic;

namespace Cocktail
{
    /// <summary>
    /// A factory that creates new instances of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of instance to be created.</typeparam>
    public interface ICompositionFactory<T> : IHideObjectMembers
         where T : class
    {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        T NewInstance();
    }

    /// <summary>
    /// A service providing implementation independent IoC functionality.
    /// </summary>
    public partial interface ICompositionProvider
    {
        /// <summary>
        ///   Returns a lazy instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        Lazy<T> GetInstance<T>() where T : class;

        /// <summary>
        ///   Returns an instance of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instance. </typeparam>
        /// <returns>Null if instance is not present in the container.</returns>
        T TryGetInstance<T>() where T : class;

        /// <summary>
        ///   Returns all instances of the specified type.
        /// </summary>
        /// <typeparam name="T"> Type of the requested instances. </typeparam>
        IEnumerable<T> GetInstances<T>() where T : class;

        /// <summary>
        ///   Returns a lazy instance of the provided type or with the specified contract name or both.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        Lazy<object> GetInstance(Type serviceType, string contractName);

        /// <summary>
        ///   Returns an instance of the specified type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        /// <returns>Null if instance is not present in the container.</returns>
        object TryGetInstance(Type serviceType, string contractName);

        /// <summary>
        ///   Returns all lazy instances of the provided type.
        /// </summary>
        /// <param name="serviceType"> The type of the requested instance. If no type is specified the contract name will be used.</param>
        /// <param name="contractName"> The contract name of the instance requested. If no contract name is specified, the type will be used. </param>
        IEnumerable<object> GetInstances(Type serviceType, string contractName);

        /// <summary>
        ///  Returns a factory that creates new instances of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of instance the factory creates.</typeparam>
        ICompositionFactory<T> GetInstanceFactory<T>() where T : class;

        /// <summary>
        ///  Returns a factory that creates new instances of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of instance the factory creates.</typeparam>
        /// <returns>Null if the container cannot provide a factory for the specified type.</returns>
        ICompositionFactory<T> TryGetInstanceFactory<T>() where T : class;

        /// <summary>Manually performs property dependency injection on the provided instance.</summary>
        /// <param name="instance">The instance needing property injection.</param>
        void BuildUp(object instance);
    }

    /// <summary>
    /// A special type of CompositionProvider that supports dynamic recompositon at runtime.
    /// </summary>
    public interface ISupportsRecomposition : ICompositionProvider
    {
        /// <summary>
        /// Returns true if the CompositionProvider is currently in the process of recomposing.
        /// </summary>
        bool IsRecomposing { get; }

        /// <summary>
        ///   Fired to indicate that a recomposition has taken place.
        /// </summary>
        event EventHandler<RecomposedEventArgs> Recomposed;
    }
}
