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
    /// <summary>
    ///   Manages and provides an EntityManager.
    /// </summary>
    /// <typeparam name="T"> The type of the EntityManager </typeparam>
    public class EntityManagerProvider<T> : EntityManagerProviderCore<T>,
                                            IHandle<SyncDataMessage<T>>, IHandle<PrincipalChangedMessage>
        where T : EntityManager
    {
        private readonly PartLocator<IEntityManagerSyncInterceptor> _syncInterceptorLocator;
        private IEnumerable<EntityKey> _deletedEntityKeys;
        private IEnumerable<EntityManagerDelegate<T>> _entityManagerDelegates;
        private IEnumerable<ISampleDataProvider<T>> _sampleDataProviders;
        private EntityCacheState _storeEcs;
        private bool _isSaving;

        private EntityManagerWrapper<T> _entityManagerWrapper;
        private IEnumerable<IValidationErrorNotification> _validationErrorNotifiers;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        public EntityManagerProvider()
        {
            _syncInterceptorLocator =
                new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.NonShared, () => CompositionContext)
                    .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());
        }

        /// <summary>
        ///   Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified <see
        ///    cref="ConnectionOptions" /> name.
        /// </summary>
        /// <param name="connectionOptionsName"> The ConnectionOptions name to be assigned. </param>
        /// <returns> A new EntityManagerProvider instance. </returns>
        public new EntityManagerProvider<T> With(string connectionOptionsName)
        {
            return (EntityManagerProvider<T>) base.With(connectionOptionsName);
        }

        /// <summary>
        ///   Creates a new EntityManagerProvider from the current EntityManagerProvider and assigns the specified sample data providers.
        /// </summary>
        /// <param name="sampleDataProviders"> The sample data providers to be assigned. </param>
        /// <returns> A new EntityManagerProvider instance. </returns>
        public EntityManagerProvider<T> With(params ISampleDataProvider<T>[] sampleDataProviders)
        {
            var newInstance = (EntityManagerProvider<T>) ((ICloneable) this).Clone();
            newInstance.SampleDataProviders = sampleDataProviders;
            return newInstance;
        }

        #region IEntityManagerProvider<T> Members

        /// <summary>
        ///   Returns the EntityManager managed by this provider.
        /// </summary>
        public override T Manager
        {
            get
            {
                if (_entityManagerWrapper == null)
                {
                    _entityManagerWrapper = new EntityManagerWrapper<T>(CreateEntityManagerCore());
                    OnManagerCreated();
                }
                return _entityManagerWrapper.Manager;
            }
        }

        /// <summary>
        ///   Returns true if a save is in progress. A <see cref="InvalidOperationException" /> is thrown if EntityManager.SaveChangesAsync is called while a previous SaveChangesAsync is still in progress.
        /// </summary>
        public override bool IsSaving
        {
            get { return _isSaving; }
        }

        #endregion

        /// <summary>
        ///   Creates a copy of the current EntityManagerProvider.
        /// </summary>
        public override object Clone()
        {
            var newInstance = (EntityManagerProvider<T>) base.Clone();
            newInstance._sampleDataProviders = _sampleDataProviders;
            return newInstance;
        }

        #region IHandle<SyncDataMessage<T>> Members

        /// <summary>
        ///   Internal use.
        /// </summary>
        void IHandle<SyncDataMessage<T>>.Handle(SyncDataMessage<T> syncData)
        {
            if (syncData.IsSameProviderAs(this)) return;

            // Merge deletions
            var removers =
                syncData.DeletedEntityKeys.Select(key => Manager.FindEntity(key, false)).Where(
                    entity => entity != null).ToList();
            if (removers.Any()) Manager.RemoveEntities(removers);

            // Merge saved entities
            var interceptor = GetSyncInterceptor();
            var mergers = syncData.SavedEntities.Where(interceptor.ShouldImportEntity);
            Manager.ImportEntities(mergers, MergeStrategy.PreserveChangesUpdateOriginal);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

        #endregion

        internal OperationResult ResetFakeBackingStoreAsync()
        {
            if (!FakeBackingStore.Exists(CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            // Create a separate isolated EntityManager
            var manager = CreateEntityManager();
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
            var manager = CreateEntityManager();
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
            EventFns.Subscribe(this);
            DiscoverAndHoldEntityManagerDelegates();
            var manager = CreateEntityManager();

            if (ConnectionOptions.IsDesignTime)
            {
                manager.Fetching +=
                    delegate { throw new InvalidOperationException(StringResources.ManagerTriedToFetchData); };
                manager.Saving +=
                    delegate { throw new InvalidOperationException(StringResources.ManagerTriedToSaveData); };

                if (SampleDataProviders != null)
                    SampleDataProviders.ForEach(p => p.AddSampleData(manager));
            }

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

        void IHandle<PrincipalChangedMessage>.Handle(PrincipalChangedMessage message)
        {
            // Let's clear the cache from the previous user and
            // release the EntityManager. A new EntityManager will
            // automatically be created and linked to the new
            // security context.
            if (_entityManagerWrapper != null)
            {
                _entityManagerWrapper.Manager.Clear();
                _entityManagerWrapper.Manager.Disconnect();
                _entityManagerWrapper.Manager.Querying +=
                    delegate { throw new InvalidOperationException(StringResources.InvalidUseOfExpiredEntityManager); };
                _entityManagerWrapper.Manager.Saving +=
                    delegate { throw new InvalidOperationException(StringResources.InvalidUseOfExpiredEntityManager); };
            }
            _entityManagerWrapper = null;
        }

        private IEntityManagerSyncInterceptor GetSyncInterceptor()
        {
            var syncInterceptor = _syncInterceptorLocator.GetPart();

            // If custom implementation, set EntityManager property
            if (syncInterceptor is EntityManagerSyncInterceptor)
                ((EntityManagerSyncInterceptor) syncInterceptor).EntityManager = Manager;

            return syncInterceptor;
        }

        private void OnSaved(EntitySavedEventArgs e)
        {
            try
            {
                if (!e.HasError)
                {
                    var interceptor = GetSyncInterceptor();
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
                _isSaving = false;
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

        private void OnSaving(EntitySavingEventArgs e)
        {
            if (IsSaving)
                throw new InvalidOperationException(
                    StringResources.ThisEntityManagerIsCurrentlyBusyWithAPreviousSaveChangeAsync);
            _isSaving = true;

            try
            {
                Validate(e);
                if (e.Cancel)
                {
                    _isSaving = false;
                    return;
                }

                var interceptor = GetSyncInterceptor();
                var syncEntities =
                    e.Entities.Where(interceptor.ShouldExportEntity);
                RetainDeletedEntityKeys(syncEntities);
            }
            catch (Exception)
            {
                _isSaving = false;
                throw;
            }
        }

        private void Validate(EntitySavingEventArgs args)
        {
            var allValidationErrors = new VerifierResultCollection();

            foreach (var entity in args.Entities)
            {
                var entityAspect = EntityAspect.Wrap(entity);
                if (entityAspect.EntityState.IsDeletedOrDetached()) continue;

                var validationErrors = Manager.VerifierEngine.Execute(entity);
                foreach (var d in _entityManagerDelegates ?? new EntityManagerDelegate<T>[0])
                    d.Validate(entity, validationErrors);
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
        }

        private void RetainDeletedEntityKeys(IEnumerable<object> syncEntities)
        {
            _deletedEntityKeys =
                syncEntities.Where(e => EntityAspect.Wrap(e).EntityState.IsDeleted()).Select(
                    e => EntityAspect.Wrap(e).EntityKey).ToList();
        }

        private void RaiseDataChangedEvent(IEnumerable<object> savedEntities, IEnumerable<EntityKey> deletedEntityKeys)
        {
            var entityKeys =
                savedEntities.Select(e => EntityAspect.Wrap(e).EntityKey).Concat(deletedEntityKeys).ToList();

            if (!entityKeys.Any()) return;

            OnDataChanged(entityKeys);
        }

        private CompositionContext CompositionContext
        {
            get { return ConnectionOptions.CompositionContext; }
        }

        private void DiscoverAndHoldEntityManagerDelegates()
        {
            if (_entityManagerDelegates != null) return;

            var i = CompositionContext.GetExportedInstances(typeof(EntityManagerDelegate));
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

            // Append internal delegate to the list of delegates
            _entityManagerDelegates = _entityManagerDelegates.Concat(new InternalDelegate(this)).ToList();
        }

        private IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers
        {
            get
            {
                if (_validationErrorNotifiers != null) return _validationErrorNotifiers;

                var i = CompositionContext.GetExportedInstances(typeof(IValidationErrorNotification));
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

        [PartNotDiscoverable]
        private class InternalDelegate : EntityManagerDelegate<T>
        {
            private readonly EntityManagerProvider<T> _entityManagerProvider;

            public InternalDelegate(EntityManagerProvider<T> entityManagerProvider)
            {
                _entityManagerProvider = entityManagerProvider;
            }

            private bool IsSameEntityManager(EntityManager manager)
            {
                return _entityManagerProvider._entityManagerWrapper != null &&
                       ReferenceEquals(_entityManagerProvider._entityManagerWrapper.Manager, manager);
            }

            public override void OnQuerying(T source, EntityQueryingEventArgs args)
            {
                if (!IsSameEntityManager(source)) return;

                // In design mode all queries must be forced to execute against the cache.
                if (Execute.InDesignMode)
                    args.Query = args.Query.With(QueryStrategy.CacheOnly);
            }

            public override void OnSaving(T source, EntitySavingEventArgs args)
            {
                if (!IsSameEntityManager(source)) return;

                _entityManagerProvider.OnSaving(args);
            }

            public override void OnSaved(T source, EntitySavedEventArgs args)
            {
                if (!IsSameEntityManager(source)) return;

                _entityManagerProvider.OnSaved(args);
            }
        }
    }
}