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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///   Interface to be implemented by a unit of work.
    /// </summary>
    public interface IUnitOfWork : IHideObjectMembers
    {
        /// <summary>
        /// Resets the UnitOfWork to its initial state.
        /// </summary>
        void Clear();

        /// <summary>
        ///   Returns true if the unit of work contains pending changes.
        /// </summary>
        bool HasChanges();

        /// <summary>
        /// Returns true if the provided entity is attached to the current UnitOfWork's EntityManager.
        /// </summary>
        /// <param name="entity">Entity to check if attached to current UnitOfWork.</param>
        bool HasEntity(object entity);

        /// <summary>
        ///   Commits all pending changes to the underlying data source.
        /// </summary>
        /// <param name="onSuccess"> Callback to be called if the commit was successful. </param>
        /// <param name="onFail"> Callback to be called if the commit failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<SaveResult> CommitAsync(Action<SaveResult> onSuccess = null, Action<Exception> onFail = null);

        /// <summary>
        ///   Rolls back all pending changes.
        /// </summary>
        void Rollback();

        /// <summary>
        ///   Fired whenever an entity associated with the current unit of work has changed in any significant manner.
        /// </summary>
        event EventHandler<EntityChangedEventArgs> EntityChanged;
    }

    /// <summary>
    ///   Interface to be implemented by a simple unit of work with a single entity.
    /// </summary>
    /// <typeparam name="T"> The type of entity used with this unit of work. </typeparam>
    public interface IUnitOfWork<T> : IUnitOfWork where T : class
    {
        /// <summary>
        ///   The factory to create new entity instances.
        /// </summary>
        IFactory<T> Factory { get; }

        /// <summary>
        ///   The repository to retrieve entities.
        /// </summary>
        IRepository<T> Entities { get; }
    }
}