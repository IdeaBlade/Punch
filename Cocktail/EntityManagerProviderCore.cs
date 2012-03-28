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
using System.ComponentModel;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///   Implements the core functionality of an EntityManagerProvider.
    /// </summary>
    /// <typeparam name="T"> The type of the EntityManager </typeparam>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class EntityManagerProviderCore<T> : IEntityManagerProvider<T>, ICloneable
        where T : EntityManager
    {
        private string _connectionOptionsName;

        #region ICloneable Members

        /// <summary>
        ///   Creates a copy of the current EntityManagerProvider.
        /// </summary>
        public virtual object Clone()
        {
            try
            {
                var newInstance = (EntityManagerProviderCore<T>) Activator.CreateInstance(GetType());
                newInstance._connectionOptionsName = _connectionOptionsName;
                return newInstance;
            }
            catch (MissingMethodException)
            {
                throw new MissingMethodException(string.Format(StringResources.MissingDefaultConstructor, GetType().Name));
            }
        }

        #endregion

        #region IEntityManagerProvider<T> Members

        /// <summary>
        ///   Specifies the ConnectionOptions used by the current EntityManagerProvider.
        /// </summary>
        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.GetByName(_connectionOptionsName); }
        }

        /// <summary>
        ///   Returns the EntityManager managed by this provider.
        /// </summary>
        public abstract T Manager { get; }

        EntityManager IEntityManagerProvider.Manager
        {
            get { return Manager; }
        }

        /// <summary>
        ///   Event fired after the EntityManager got created.
        /// </summary>
        public event EventHandler<EntityManagerCreatedEventArgs> ManagerCreated = delegate { };

        /// <summary>
        ///   Returns true if any entities in the EntityManager cache have validation errors.
        /// </summary>
        public bool HasValidationError
        {
            get
            {
                return Manager.FindEntities<object>(EntityState.AllButDetached)
                    .Select(EntityAspect.Wrap)
                    .Any(a => a.ValidationErrors.HasErrors);
            }
        }

        /// <summary>
        ///   Returns true if a save is in progress. A <see cref="InvalidOperationException" /> is thrown if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        public abstract bool IsSaving { get; }

        /// <summary>
        ///   Signals that a Save of at least one entity has been performed or changed entities have been imported from another entity manager. Clients may use this event to force a data refresh.
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged;

        #endregion

        /// <summary>
        ///   Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified <see
        ///    cref="ConnectionOptions" /> name.
        /// </summary>
        /// <param name="connectionOptionsName"> The ConnectionOptions name to be assigned. </param>
        /// <returns> A new EntityManagerProvider instance. </returns>
        protected EntityManagerProviderCore<T> With(string connectionOptionsName)
        {
            var newInstance = (EntityManagerProviderCore<T>) ((ICloneable) this).Clone();
            newInstance._connectionOptionsName = connectionOptionsName;
            return newInstance;
        }

        /// <summary>
        ///   Triggers the ManagerCreated event.
        /// </summary>
        protected virtual void OnManagerCreated()
        {
            ManagerCreated(this, new EntityManagerCreatedEventArgs(Manager));
        }

        /// <summary>
        ///   Creates a new EntityManager instance.
        /// </summary>
        protected virtual T CreateEntityManager()
        {
            try
            {
                var connectionOptions = ConnectionOptions;
                var manager = (T) Activator.CreateInstance(typeof(T), connectionOptions.ToEntityManagerContext());
                DebugFns.WriteLine(string.Format(StringResources.SuccessfullyCreatedEntityManager,
                                                 manager.GetType().FullName, connectionOptions.Name,
                                                 connectionOptions.IsFake));
                return manager;
            }
            catch (MissingMethodException)
            {
                throw new MissingMethodException(string.Format(StringResources.MissingEntityManagerConstructor,
                                                               typeof(T).Name));
            }
        }

        /// <summary>
        ///   Triggers the DataChanged event.
        /// </summary>
        /// <param name="entityKeys"> The list of keys for the changed entities </param>
        protected void OnDataChanged(IEnumerable<EntityKey> entityKeys)
        {
            if (DataChanged != null)
                DataChanged(this, new DataChangedEventArgs(entityKeys, Manager));
        }
    }
}