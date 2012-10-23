//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using Caliburn.Micro;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;
using IdeaBlade.Core.Composition;

namespace Cocktail
{
    /// <summary>Internal use.</summary>
    [InterfaceExport(typeof(EntityManagerDelegate))]
    public abstract class EntityManagerDelegate
    {
    }

    /// <summary>Provides the means to perform tasks in response to events from an EntityManager.</summary>
    /// <typeparam name="T">The EntityManager type for which the events should be intercepted.</typeparam>
    /// <remarks>To handle events, create a new class extending EntityManagerDelegate&lt;T&gt; and override the respective methods. Multiple
    /// EntityManagerDelegates are supported.</remarks>
    public abstract class EntityManagerDelegate<T> : EntityManagerDelegate, IHandle<EntityManagerEventMessage<T>>
        where T : EntityManager
    {
        /// <summary>
        /// Initializes a new EntityManagerDelegate instance.
        /// </summary>
        protected EntityManagerDelegate()
        {
            EventFns.Subscribe(this);
        }

        #region IHandle<EntityManagerEventMessage<T>> Members

        void IHandle<EntityManagerEventMessage<T>>.Handle(EntityManagerEventMessage<T> message)
        {
            var entityManagerClearedEventArgs = message.EventArgs as EntityManagerClearedEventArgs;
            if (entityManagerClearedEventArgs != null)
                OnCleared(message.EntityManager, entityManagerClearedEventArgs);
            var entityChangedEventArgs = message.EventArgs as EntityChangedEventArgs;
            if (entityChangedEventArgs != null)
                OnEntityChanged(message.EntityManager, entityChangedEventArgs);
            var entityChangingEventArgs = message.EventArgs as EntityChangingEventArgs;
            if (entityChangingEventArgs != null)
                OnEntityChanging(message.EntityManager, entityChangingEventArgs);
            var entityServerErrorEventArgs = message.EventArgs as EntityServerErrorEventArgs;
            if (entityServerErrorEventArgs != null)
                OnEntityServerError(message.EntityManager, entityServerErrorEventArgs);
            var entityFetchingEventArgs = message.EventArgs as EntityFetchingEventArgs;
            if (entityFetchingEventArgs != null)
                OnFetching(message.EntityManager, entityFetchingEventArgs);
            var entityQueriedEventArgs = message.EventArgs as EntityQueriedEventArgs;
            if (entityQueriedEventArgs != null)
                OnQueried(message.EntityManager, entityQueriedEventArgs);
            var entityQueryingEventArgs = message.EventArgs as EntityQueryingEventArgs;
            if (entityQueryingEventArgs != null)
                OnQuerying(message.EntityManager, entityQueryingEventArgs);
            var entitySavingEventArgs = message.EventArgs as EntitySavingEventArgs;
            if (entitySavingEventArgs != null)
                OnSaving(message.EntityManager, entitySavingEventArgs);
            var entitySavedEventArgs = message.EventArgs as EntitySavedEventArgs;
            if (entitySavedEventArgs != null)
                OnSaved(message.EntityManager, entitySavedEventArgs);
        }

        #endregion

        /// <summary>Called whenever the entityManager is cleared.</summary>
        /// <param name="source">The EntityManager on which a EntityManager.Clear() has been called.</param>
        /// <param name="args">The original event arguments.</param>
        public virtual void OnCleared(T source, EntityManagerClearedEventArgs args)
        {
        }

        /// <summary>Called whenever an entity's state has changed in any significant manner.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnEntityChanged(T source, EntityChangedEventArgs args)
        {
        }

        /// <summary>Called whenever an entity's state is changing in any significant manner.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnEntityChanging(T source, EntityChangingEventArgs args)
        {
        }

        /// <summary>Called when an error occurs while accessing the EntityServer or backend data source.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnEntityServerError(T source, EntityServerErrorEventArgs args)
        {
        }

        /// <summary>Called before the EntityManager fetches data from an EntityServer. Will only be called if the query will not be satisfied out of
        /// the local cache.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnFetching(T source, EntityFetchingEventArgs args)
        {
        }

        /// <summary>Occurs after the EntityManager has completed processing of a query.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnQueried(T source, EntityQueriedEventArgs args)
        {
        }

        /// <summary>Called before the EntityManager executes a query.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnQuerying(T source, EntityQueryingEventArgs args)
        {
        }

        /// <summary>Called after the EntityManager has persisted changed entities.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnSaved(T source, EntitySavedEventArgs args)
        {
        }

        /// <summary>Called when the EntityManager is preparing to save changes.</summary>
        /// <param name="args">The original event arguments.</param>
        /// <param name="source">The source of the event.</param>
        public virtual void OnSaving(T source, EntitySavingEventArgs args)
        {
        }

        /// <summary>
        /// Override to perform client-side custom validation on a given entity before saving.
        /// </summary>
        /// <param name="entity">The entity to be validated</param>
        /// <param name="validationErrors">A collection to add the validation results</param>
        public virtual void Validate(object entity, VerifierResultCollection validationErrors)
        {
        }
    }
}