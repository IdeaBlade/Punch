//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace Cocktail
{
    /// <summary>Base class for an EntityMangerProvider.</summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public abstract class EntityManagerProviderBase<T> : IEntityManagerProvider<T>,
                                                         IHandle<SyncDataMessage<T>>
        where T : EntityManager
    {
        private EventDispatcher<T> _eventDispatcher;
        private IEnumerable<EntityKey> _deletedEntityKeys;
        private IEnumerable<IValidationErrorNotification> _validationErrorNotifiers;
        private IEnumerable<EntityManagerDelegate<T>> _entityManagerInterceptors;

        private readonly PartLocator<IEventAggregator> _eventAggregatorLocator;
        private readonly PartLocator<IAuthenticationService> _authenticationServiceLocator;
        private readonly PartLocator<IEntityManagerSyncInterceptor> _syncInterceptorLocator;

        private T _manager;

        /// <summary>Initializes a new instance.</summary>
        protected EntityManagerProviderBase()
        {
            _eventAggregatorLocator =
                new PartLocator<IEventAggregator>(CreationPolicy.Shared, () => Manager.CompositionContext);
            _syncInterceptorLocator =
                new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.NonShared, () => Manager.CompositionContext)
                    .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());
            _authenticationServiceLocator
                = new PartLocator<IAuthenticationService>(CreationPolicy.Shared, () => Manager.CompositionContext)
                    .WithInitializer(a => EventDispatcher.InstallEventHandlers(a));
        }

        private EventDispatcher<T> EventDispatcher
        {
            get
            {
                if (_eventDispatcher == null)
                {
                    _eventDispatcher = new EventDispatcher<T>(EntityManagerDelegates);
                    _eventDispatcher.PrincipalChanged += OnPrincipalChanged;
                    _eventDispatcher.Querying += OnQuerying;
                    _eventDispatcher.Saving += OnSaving;
                    _eventDispatcher.Saved += OnSaved;
                }

                return _eventDispatcher;
            }
        }

        private IEnumerable<EntityManagerDelegate<T>> EntityManagerDelegates
        {
            get
            {
                if (_entityManagerInterceptors != null) return _entityManagerInterceptors;

                if (!Composition.IsConfigured) return new EntityManagerDelegate<T>[0];

                var i = Manager.CompositionContext.GetExportedInstances(typeof(EntityManagerDelegate));
                if (i != null)
                    _entityManagerInterceptors = i.OfType<EntityManagerDelegate<T>>().ToList();

                if (_entityManagerInterceptors == null || !_entityManagerInterceptors.Any())
                    _entityManagerInterceptors = Composition.GetInstances<EntityManagerDelegate>()
                        .OfType<EntityManagerDelegate<T>>()
                        .ToList();

                TraceFns.WriteLine(_entityManagerInterceptors.Any()
                                       ? string.Format(StringResources.ProbedForEntityManagerDelegateAndFoundMatch,
                                                       _entityManagerInterceptors.Count())
                                       : StringResources.ProbedForEntityManagerDelegateAndFoundNoMatch);

                return _entityManagerInterceptors;
            }
        }

        private IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers
        {
            get
            {
                if (_validationErrorNotifiers != null) return _validationErrorNotifiers;

                if (!Composition.IsConfigured) return new IValidationErrorNotification[0];

                var i = Manager.CompositionContext.GetExportedInstances(typeof(IValidationErrorNotification));
                if (i != null)
                    _validationErrorNotifiers = i.Cast<IValidationErrorNotification>().ToList();

                if (_validationErrorNotifiers == null || !_validationErrorNotifiers.Any())
                    _validationErrorNotifiers = Composition.GetInstances<IValidationErrorNotification>().ToList();

                TraceFns.WriteLine(_validationErrorNotifiers.Any()
                                       ? string.Format(
                                           StringResources.ProbedForIValidationErrorNotificationAndFoundMatch,
                                           _validationErrorNotifiers.Count())
                                       : StringResources.ProbedForIValidationErrorNotificationAndFoundNoMatch);

                return _validationErrorNotifiers;
            }
        }

        private IAuthenticationService AuthenticationService
        {
            get { return _authenticationServiceLocator.GetPart(); }
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        private void OnPrincipalChanged(object sender, EventArgs args)
        {
            // Let's clear the cache from the previous user and
            // release the EntityManager. A new EntityManager will
            // automatically be created and linked to the new
            // security context.
            if (_manager != null)
            {
                _manager.Clear();
                _manager.Disconnect();
                _manager.Querying +=
                    delegate { throw new InvalidOperationException(StringResources.InvalidUseOfExpiredEntityManager); };
                _manager.Saving +=
                    delegate { throw new InvalidOperationException(StringResources.InvalidUseOfExpiredEntityManager); };
            }
            _manager = null;
        }

        #region IEntityManagerProvider<T> Members

        /// <summary>Returns the EntityManager managed by this provider.</summary>
        public T Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = CreateEntityManagerCore();
                    LinkAuthentication(_manager);
                    EventDispatcher.InstallEventHandlers(_manager);
                    if (EventAggregator != null)
                        EventAggregator.Subscribe(this);

                    OnManagerCreated();
                }
                return _manager;
            }
        }

        /// <summary>
        /// Triggers the ManagerCreated event.
        /// </summary>
        protected virtual void OnManagerCreated()
        {
            ManagerCreated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event fired after the EntityManager got created.
        /// </summary>
        public event EventHandler<EventArgs> ManagerCreated = delegate { };

        /// <summary>Performs the necessary initialization steps for the persistence layer. The specific steps depend on the subtype of EntityManagerProvider used.</summary>
        public virtual OperationResult InitializeAsync()
        {
            return AlwaysCompletedOperationResult.Instance;
        }

        /// <summary>
        /// Returns true if the last save operation aborted due to a validation error.
        /// </summary>
        public bool HasValidationError { get; private set; }

        /// <summary>
        /// Returns true if a save is in progress. A <see cref="InvalidOperationException"/> is thrown 
        /// if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        public bool IsSaving { get; private set; }

#if !SILVERLIGHT
    /// <summary>Performs the necessary initialization steps for the persistence layer. The specific steps depend on the subtype of EntityManagerProvider used.</summary>
        public virtual void Initialize()
        {
        }

#endif

        /// <summary>Indicates whether the persistence layer has been properly initialized.</summary>
        public virtual bool IsInitialized
        {
            get { return true; }
        }

        #endregion

        /// <summary>Internal use.</summary>
        protected virtual T CreateEntityManagerCore()
        {
            if (Composition.IsRecomposing)
                throw new InvalidOperationException(StringResources.CreatingEntityManagerDuringRecompositionNotAllowed);

            if (Composition.IsConfigured)
                Composition.BuildUp(this);

            T manager = CreateEntityManager();
            return manager;
        }

        /// <summary>Internal use.</summary>
        protected bool LinkAuthentication(T manager)
        {
            return _authenticationServiceLocator.IsAvailable && AuthenticationService.LinkAuthentication(manager);
        }

        /// <summary>
        /// Creates the EntityManager to be used for authentication.
        /// </summary>
        protected internal T CreateAuthenticationManager()
        {
            return CreateEntityManager();
        }

        /// <summary>Internal use.</summary>
        private void OnQuerying(object sender, EntityQueryingEventArgs e)
        {
            MustBeInitialized();

            // In design mode all queries must be forced to execute against the cache.
            if (Execute.InDesignMode)
                e.Query = e.Query.With(QueryStrategy.CacheOnly);
        }

        /// <summary>Throws an exception if the EntityManagerProvider is not initialized.</summary>
        /// <exception caption="" cref="System.InvalidOperationException">Thrown if not initialized.</exception>
        protected void MustBeInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException(StringResources.TheEntityManagerProviderHasNotBeenInitialized);
        }

        /// <summary>
        /// Overload to instantiate the concrete type of the EntityManager
        /// </summary>
        /// <returns>T</returns>
        protected abstract T CreateEntityManager();

        #region Data saving and synchronization logic

        private IEventAggregator EventAggregator
        {
            get { return _eventAggregatorLocator.GetPart(); }
        }

        private IEntityManagerSyncInterceptor GetSyncInterceptor()
        {
            IEntityManagerSyncInterceptor syncInterceptor = _syncInterceptorLocator.GetPart();

            // If custom implementation, set EntityManager property
            if (syncInterceptor is EntityManagerSyncInterceptor)
                ((EntityManagerSyncInterceptor)syncInterceptor).EntityManager = Manager;

            return syncInterceptor;
        }

        /// <summary>Internal use.</summary>
        void IHandle<SyncDataMessage<T>>.Handle(SyncDataMessage<T> syncData)
        {
            if (syncData.IsSameProviderAs(this)) return;

            // Merge deletions
            var removers =
                syncData.DeletedEntityKeys.Select(key => Manager.FindEntity(key, false)).Where(
                    entity => entity != null).ToList();
            if (removers.Any()) Manager.RemoveEntities(removers);

            // Merge saved entities
            IEntityManagerSyncInterceptor interceptor = GetSyncInterceptor();
            var mergers = syncData.SavedEntities.Where(interceptor.ShouldImportEntity);
            Manager.ImportEntities(mergers, MergeStrategy.PreserveChangesUpdateOriginal);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

        /// <summary>Internal use.</summary>
        private void OnSaved(object sender, EntitySavedEventArgs e)
        {
            try
            {
                if (!e.HasError)
                {
                    IEntityManagerSyncInterceptor interceptor = GetSyncInterceptor();
                    var exportEntities =
                        e.Entities.Where(
                            entity =>
                            interceptor.ShouldExportEntity(entity) &&
                            !_deletedEntityKeys.Contains(EntityAspect.Wrap(entity).EntityKey)).ToList();
                    PublishEntities(exportEntities);
                }
                _deletedEntityKeys = null;
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void PublishEntities(IEnumerable<object> exportEntities)
        {
            if (EventAggregator == null) return;

            var syncData = new SyncDataMessage<T>(this, exportEntities, _deletedEntityKeys);
            EventAggregator.Publish(syncData);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

        /// <summary>Internal use.</summary>
        private void OnSaving(object sender, EntitySavingEventArgs e)
        {
            if (IsSaving)
                throw new InvalidOperationException(
                    StringResources.ThisEntityManagerIsCurrentlyBusyWithAPreviousSaveChangeAsync);
            IsSaving = true;

            try
            {
                Validate(e);
                if (e.Cancel)
                {
                    IsSaving = false;
                    return;
                }

                IEntityManagerSyncInterceptor interceptor = GetSyncInterceptor();
                var syncEntities =
                    e.Entities.Where(interceptor.ShouldExportEntity);
                RetainDeletedEntityKeys(syncEntities);
            }
            catch (Exception)
            {
                IsSaving = false;
                throw;
            }
        }

        private void Validate(EntitySavingEventArgs args)
        {
            if (!Composition.IsConfigured) return;

            var allValidationErrors = new VerifierResultCollection();

            foreach (var entity in args.Entities)
            {
                var entityAspect = EntityAspect.Wrap(entity);
                if (entityAspect.EntityState.IsDeletedOrDetached()) continue;

                var validationErrors = Manager.VerifierEngine.Execute(entity);
                foreach (var i in EntityManagerDelegates)
                    i.Validate(entity, validationErrors);
                // Extract only validation errors
                validationErrors = validationErrors.Errors;

                validationErrors.Where(vr => !entityAspect.ValidationErrors.Contains(vr))
                    .ForEach(entityAspect.ValidationErrors.Add);

                validationErrors.ForEach(allValidationErrors.Add);
            }

            if (allValidationErrors.HasErrors)
            {
                if (!ValidationErrorNotifiers.Any())
                    throw new ValidationException(allValidationErrors.Select(v => v.Message).ToAggregateString("\n"));

                ValidationErrorNotifiers.ForEach(s => s.OnValidationError(allValidationErrors));
                args.Cancel = true;
            }

            HasValidationError = args.Cancel;
        }

        private void RetainDeletedEntityKeys(IEnumerable<object> syncEntities)
        {
            _deletedEntityKeys =
                syncEntities.Where(e => EntityAspect.Wrap(e).EntityState.IsDeleted()).Select(
                    e => EntityAspect.Wrap(e).EntityKey).ToList();
        }

        /// <summary>
        /// Signals that a Save of at least one entity has been performed
        /// or changed entities have been imported from another entity manager.
        /// Clients may use this event to force a data refresh. 
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// Internal use.
        /// </summary>
        /// <param name="savedEntities"></param>
        /// <param name="deletedEntityKeys"></param>
        protected void RaiseDataChangedEvent(IEnumerable<object> savedEntities, IEnumerable<EntityKey> deletedEntityKeys)
        {
            List<EntityKey> entityKeys =
                savedEntities.Select(e => EntityAspect.Wrap(e).EntityKey).Concat(deletedEntityKeys).ToList();

            if (!entityKeys.Any()) return;

            var args = new DataChangedEventArgs(entityKeys, Manager);
            if (DataChanged != null) DataChanged(this, args);
        }

        #endregion
    }
}