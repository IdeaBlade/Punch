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
using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///   Base implementation of a unit of work.
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly IEntityManagerProvider _entityManagerProvider;

        /// <summary>
        ///   Creates a new unit of work.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        protected UnitOfWork(IEntityManagerProvider entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
            _entityManagerProvider.EntityChanged += new EventHandler<EntityChangedEventArgs>(OnEntityChanged)
                .MakeWeak(eh => _entityManagerProvider.EntityChanged -= eh);
        }

        /// <summary>
        ///   Returns the EntityManager used by this unit of work.
        /// </summary>
        protected EntityManager EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IUnitOfWork Members

        /// <summary>
        /// Resets the UnitOfWork to its initial state.
        /// </summary>
        public void Clear()
        {
            EntityManager.Clear();
        }

        /// <summary>
        ///   Returns true if the unit of work contains pending changes.
        /// </summary>
        public bool HasChanges()
        {
            return EntityManager.HasChanges();
        }

        /// <summary>
        /// Returns true if the provided entity is attached to the current UnitOfWork's EntityManager.
        /// </summary>
        /// <param name="entity">Entity to check if attached to current UnitOfWork.</param>
        public bool HasEntity(object entity)
        {
            var entityAspect = EntityAspect.Wrap(entity);
            return !entityAspect.EntityState.IsDetached() && EntityManager == entityAspect.EntityManager;
        }

        /// <summary>
        ///   Commits all pending changes to the underlying data source.
        /// </summary>
        public async virtual Task<SaveResult> CommitAsync()
        {
            var saveResult = await EntityManager.TrySaveChangesAsync();

            if (saveResult.WasCancelled)
                throw new TaskCanceledException();
            
            if (saveResult.Error != null && !saveResult.WasExceptionHandled)
                throw saveResult.Error;

            return saveResult;
        }

        /// <summary>
        ///   Rolls back all pending changes.
        /// </summary>
        public virtual void Rollback()
        {
            EntityManager.RejectChanges();
        }

        /// <summary>
        ///   Fired whenever an entity associated with the current unit of work has changed in any significant manner.
        /// </summary>
        public event EventHandler<EntityChangedEventArgs> EntityChanged;

        #endregion

        internal void OnEntityChanged(object sender, EntityChangedEventArgs args)
        {
            if (EntityChanged != null)
                EntityChanged(this, args);
        }
    }

    /// <summary>
    ///   A simple unit of work implementation for a single entity type.
    /// </summary>
    /// <typeparam name="T"> The type of entity. </typeparam>
    public class UnitOfWork<T> : UnitOfWork, IUnitOfWork<T> where T : class
    {
        /// <summary>
        ///   Creates a new unit of work.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        public UnitOfWork(IEntityManagerProvider entityManagerProvider)
            : base(entityManagerProvider)
        {
            Factory = new Factory<T>(entityManagerProvider);
            Entities = new Repository<T>(entityManagerProvider);
        }

        #region IUnitOfWork<T> Members

        /// <summary>
        ///   The factory to create new entity instances.
        /// </summary>
        public IFactory<T> Factory { get; private set; }

        /// <summary>
        ///   The repository to retrieve entities.
        /// </summary>
        public IRepository<T> Entities { get; private set; }

        #endregion
    }
}