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
using IdeaBlade.Core;

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
        /// <param name="onSuccess">Callback to be called if the entity creation was successful.</param>
        /// <param name="onFail">Callback to be called if the entity creation failed.</param>
        /// <returns>Asynchronous operation result.</returns>
        OperationResult<T> CreateAsync(Action<T> onSuccess = null, Action<Exception> onFail = null);
    }
}