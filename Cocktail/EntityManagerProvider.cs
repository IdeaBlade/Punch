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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace Cocktail
{
    /// <summary>Manages and provides an EntityManager.</summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public class EntityManagerProvider<T> : IEntityManagerProvider<T>, IHandle<SyncDataMessage<T>>, ICloneable
        where T : EntityManager
    {
        private readonly PartLocator<IEntityManagerSyncInterceptor> _syncInterceptorLocator;
        private string _connectionOptionsName;
        private IEnumerable<EntityKey> _deletedEntityKeys;
        private IEnumerable<EntityManagerDelegate<T>> _entityManagerDelegates;
        private EventDispatcher<T> _eventDispatcher;
        private IEnumerable<ISampleDataProvider<T>> _sampleDataProviders;
        private EntityCacheState _storeEcs;

        private T _manager;
        private IEnumerable<IValidationErrorNotification> _validationErrorNotifiers;

        /// <summary>Initializes a new instance.</summary>
        public EntityManagerProvider()
        {
            _syncInterceptorLocator =
                new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.NonShared, () => CompositionContext)
                    .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());
        }

        /// <summary>
        /// Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified <see cref="ConnectionOptions"/> name.
        /// </summary>
        /// <param name="connectionOptionsName">The ConnectionOptions name to be assigned.</param>
        /// <returns>A new EntityManagerProvider instance.</returns>
        public EntityManagerProvider<T> With(string connectionOptionsName)
        {
            var newInstance = (EntityManagerProvider<T>) ((ICloneable) this).Clone();
            newInstance._connectionOptionsName = connectionOptionsName;
            return newInstance;
        }

        /// <summary>
        /// Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified sample data providers.
        /// </summary>
        /// <param name="sampleDataProviders">The sample data providers to be assigned.</param>
        /// <returns>A new EntityManagerProvider instance.</returns>
        public EntityManagerProvider<T> With(params ISampleDataProvider<T>[] sampleDataProviders)
        {
            var newInstance = (EntityManagerProvider<T>) ((ICloneable) this).Clone();
            newInstance.SampleDataProviders = sampleDataProviders;
            return newInstance;
        }

        #region IEntityManagerProvider<T> Members

        /// <summary>
        /// Specifies the ConnectionOptions used by the current EntityManagerProvider.
        /// </summary>
        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.GetByName(_connectionOptionsName); }
        }

        /// <summary>Returns the EntityManager managed by this provider.</summary>
        public T Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = CreateEntityManagerCore();
                    OnManagerCreated();
                }
                return _manager;
            }
        }

        EntityManager IEntityManagerProvider.Manager
        {
            get { return Manager; }
        }

        /// <summary>
        /// Event fired after the EntityManager got created.
        /// </summary>
        public event EventHandler<EntityManagerCreatedEventArgs> ManagerCreated = delegate { };

        /// <summary>
        /// Returns true if the last save operation aborted due to a validation error.
        /// </summary>
        public bool HasValidationError { get; private set; }

        /// <summary>
        /// Returns true if a save is in progress. A <see cref="InvalidOperationException"/> is thrown 
        /// if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        public bool IsSaving { get; private set; }

        /// <summary>
        /// Signals that a Save of at least one entity has been performed
        /// or changed entities have been imported from another entity manager.
        /// Clients may use this event to force a data refresh. 
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged;

        #endregion

        #region IHandle<SyncDataMessage<T>> Members

        /// <summary>Internal use.</summary>
        void IHandle<SyncDataMessage<T>>.Handle(SyncDataMessage<T> syncData)
        {
            if (syncData.IsSameProviderAs(this)) return;

            // Merge deletions
            List<object> removers =
                syncData.DeletedEntityKeys.Select(key => Manager.FindEntity(key, false)).Where(
                    entity => entity != null).ToList();
            if (removers.Any()) Manager.RemoveEntities(removers);

            // Merge saved entities
            IEntityManagerSyncInterceptor interceptor = GetSyncInterceptor();
            IEnumerable<object> mergers = syncData.SavedEntities.Where(interceptor.ShouldImportEntity);
            Manager.ImportEntities(mergers, MergeStrategy.PreserveChangesUpdateOriginal);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

        #endregion

        /// <summary>
        /// Triggers the ManagerCreated event.
        /// </summary>
        protected virtual void OnManagerCreated()
        {
            ManagerCreated(this, new EntityManagerCreatedEventArgs(_manager));
        }

        /// <summary>
        /// Creates a new EntityManager instance.
        /// </summary>
        /// <returns>T</returns>
        protected virtual T CreateEntityManager()
        {
            try
            {
                ConnectionOptions connectionOptions = ConnectionOptions;
                var manager = (T) Activator.CreateInstance(typeof (T), connectionOptions.ToEntityManagerContext());
                DebugFns.WriteLine(string.Format(StringResources.SuccessfullyCreatedEntityManager,
                                                 manager.GetType().FullName, connectionOptions.Name,
                                                 connectionOptions.IsFake));
                return manager;
            }
            catch (MissingMethodException)
            {
                throw new MissingMethodException(string.Format(StringResources.MissingEntityManagerConstructor,
                                                               typeof (T).Name));
            }
        }

        internal OperationResult ResetFakeBackingStoreAsync()
        {
            if (!FakeBackingStore.Exists(CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            // Create a separate isolated EntityManager
            T manager = CreateEntityManager();
            manager.AuthenticationContext = AnonymousAuthenticationContext.Instance;

            if (_storeEcs == null)
                PopulateStoreEcs(manager);

            return FakeBackingStore.Get(CompositionContext.Name).ResetAsync(manager, _storeEcs);
        }

#if !SILVERLIGHT

        internal void ResetFakeBackingStore()
        {
            if (!FakeBackingStore.Exists(CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            // Create a separate isolated EntityManager
            T manager = CreateEntityManager();
            manager.AuthenticationContext = AnonymousAuthenticationContext.Instance;

            if (_storeEcs == null)
                PopulateStoreEcs(manager);

            FakeBackingStore.Get(CompositionContext.Name).Reset(manager, _storeEcs);
        }

#endif

        private T CreateEntityManagerCore()
        {
            if (Composition.IsRecomposing)
                throw new InvalidOperationException(StringResources.CreatingEntityManagerDuringRecompositionNotAllowed);

            Composition.BuildUp(this);

            T manager = CreateEntityManager();

            if (ConnectionOptions.IsDesignTime)
            {
                manager.Fetching +=
                    delegate { throw new InvalidOperationException(StringResources.ManagerTriedToFetchData); };
                manager.Saving +=
                    delegate { throw new InvalidOperationException(StringResources.ManagerTriedToSaveData); };

                if (SampleDataProviders != null)
                    SampleDataProviders.ForEach(p => p.AddSampleData(manager));
            }

            EventDispatcher.InstallEventHandlers(manager);

            var locator =
                new PartLocator<IAuthenticationService>(CreationPolicy.Shared, () => CompositionContext);
            if (locator.IsAvailable)
                EventDispatcher.InstallEventHandlers(locator.GetPart());

            EventFns.Subscribe(this);

            return manager;
        }

        private void PopulateStoreEcs(T manager)
        {
            if (SampleDataProviders != null)
                SampleDataProviders.ForEach(p => p.AddSampleData(manager));

            _storeEcs = manager.CacheStateManager.GetCacheState();
            // We used the manager just to get the ECS; now clear it out
            manager.Clear();
        }

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

        private void OnQuerying(object sender, EntityQueryingEventArgs e)
        {
            // In design mode all queries must be forced to execute against the cache.
            if (Execute.InDesignMode)
                e.Query = e.Query.With(QueryStrategy.CacheOnly);
        }

        private IEntityManagerSyncInterceptor GetSyncInterceptor()
        {
            IEntityManagerSyncInterceptor syncInterceptor = _syncInterceptorLocator.GetPart();

            // If custom implementation, set EntityManager property
            if (syncInterceptor is EntityManagerSyncInterceptor)
                ((EntityManagerSyncInterceptor) syncInterceptor).EntityManager = Manager;

            return syncInterceptor;
        }

        private void OnSaved(object sender, EntitySavedEventArgs e)
        {
            try
            {
                if (!e.HasError)
                {
                    IEntityManagerSyncInterceptor interceptor = GetSyncInterceptor();
                    List<object> exportEntities =
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
            var syncData = new SyncDataMessage<T>(this, exportEntities, _deletedEntityKeys);
            EventFns.Publish(syncData);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

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
                IEnumerable<object> syncEntities =
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
            var allValidationErrors = new VerifierResultCollection();

            foreach (object entity in args.Entities)
            {
                EntityAspect entityAspect = EntityAspect.Wrap(entity);
                if (entityAspect.EntityState.IsDeletedOrDetached()) continue;

                VerifierResultCollection validationErrors = Manager.VerifierEngine.Execute(entity);
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

        private void RaiseDataChangedEvent(IEnumerable<object> savedEntities, IEnumerable<EntityKey> deletedEntityKeys)
        {
            List<EntityKey> entityKeys =
                savedEntities.Select(e => EntityAspect.Wrap(e).EntityKey).Concat(deletedEntityKeys).ToList();

            if (!entityKeys.Any()) return;

            var args = new DataChangedEventArgs(entityKeys, Manager);
            if (DataChanged != null) DataChanged(this, args);
        }

        private CompositionContext CompositionContext
        {
            get { return ConnectionOptions.CompositionContext; }
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
                if (_entityManagerDelegates != null) return _entityManagerDelegates;

                IEnumerable i = CompositionContext.GetExportedInstances(typeof (EntityManagerDelegate));
                if (i != null)
                    _entityManagerDelegates = i.OfType<EntityManagerDelegate<T>>().ToList();

                if (_entityManagerDelegates == null || !_entityManagerDelegates.Any())
                    _entityManagerDelegates = Composition.GetInstances<EntityManagerDelegate>()
                        .OfType<EntityManagerDelegate<T>>()
                        .ToList();

                TraceFns.WriteLine(_entityManagerDelegates.Any()
                                       ? string.Format(StringResources.ProbedForEntityManagerDelegateAndFoundMatch,
                                                       _entityManagerDelegates.Count())
                                       : StringResources.ProbedForEntityManagerDelegateAndFoundNoMatch);

                return _entityManagerDelegates;
            }
        }

        private IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers
        {
            get
            {
                if (_validationErrorNotifiers != null) return _validationErrorNotifiers;

                IEnumerable i = CompositionContext.GetExportedInstances(typeof (IValidationErrorNotification));
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

        private IEnumerable<ISampleDataProvider<T>> SampleDataProviders
        {
            get
            {
                return _sampleDataProviders ??
                       (_sampleDataProviders = Composition.GetInstances<ISampleDataProvider<T>>());
            }
            set { _sampleDataProviders = value; }
        }

        object ICloneable.Clone()
        {
            try
            {
                var newInstance = (EntityManagerProvider<T>) Activator.CreateInstance(GetType());
                newInstance._connectionOptionsName = _connectionOptionsName;
                newInstance._sampleDataProviders = _sampleDataProviders;
                return newInstance;
            }
            catch (MissingMethodException)
            {
                throw new MissingMethodException(string.Format(StringResources.MissingDefaultConstructor, GetType().Name));
            }
        }
    }
}