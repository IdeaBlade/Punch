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

using System.Threading;
using IdeaBlade.Core.Reflection;
using IdeaBlade.EntityModel;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cocktail
{
    /// <summary>
    ///   A generic entity factory implementation
    /// </summary>
    /// <typeparam name="T"> The type of entity this factory creates. </typeparam>
    public class Factory<T> : IFactory<T> where T : class
    {
        private readonly IEntityManagerProvider _entityManagerProvider;

        /// <summary>
        ///   Creates a new factory.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        public Factory(IEntityManagerProvider entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        /// <summary>
        ///   Returns the EntityManager used by this factory.
        /// </summary>
        protected EntityManager EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IFactory<T> Members

        /// <summary>
        /// Creates a new entity instance of type T.
        /// </summary>
        /// <returns>The newly created entity attached to the underlying EntityManager.</returns>
        public Task<T> CreateAsync()
        {
            return CreateAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates a new entity instance of type T.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns>The newly created entity attached to the underlying EntityManager.</returns>
        public async virtual Task<T> CreateAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var methodInfo = FindFactoryMethod(typeof(T));
            var instance = methodInfo != null ? methodInfo.Invoke(null, new object[0]) : Activator.CreateInstance<T>();
            EntityManager.AddEntity(instance);

            return await TaskFns.FromResult((T) instance);
        }

        #endregion

        /// <summary>
        /// Locates a suitable factory method for the provided type.
        /// </summary>
        /// <param name="type">The type for which a factory method is needed.</param>
        /// <returns>Null if no suitable factory method could be located.</returns>
        protected virtual MethodInfo FindFactoryMethod(Type type)
        {
            var candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            candidates = candidates.Where(c => c.ReturnType == type && !c.GetParameters().Any()).ToArray();

            return candidates.Count() == 1 ? candidates.First() : null;
        }
    }
}