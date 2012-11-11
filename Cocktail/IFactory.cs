// ====================================================================================================================
//  Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//  WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//  OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//  OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//  USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//  http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
#if !LIGHT
using IdeaBlade.Core;
#endif

namespace Cocktail
{
    /// <summary>
    /// Interface to be implemented by an entity factory.
    /// </summary>
    /// <typeparam name="T">The type of entity this factory can create.</typeparam>
    public interface IFactory<T> : IHideObjectMembers where T : class
    {
        /// <summary>
        /// Creates a new entity instance of type T.
        /// </summary>
        /// <returns>The newly created entity attached to the underlying EntityManager.</returns>
        Task<T> CreateAsync();

        /// <summary>
        /// Creates a new entity instance of type T.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns>The newly created entity attached to the underlying EntityManager.</returns>
        Task<T> CreateAsync(CancellationToken cancellationToken);
    }
}