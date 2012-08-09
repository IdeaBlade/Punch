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
    /// Interface used to configure an EntityManagerProvider.
    /// </summary>
    /// <typeparam name="T">The type of EntityManager</typeparam>
    public interface IEntityManagerProviderConfigurator<T> : IHideObjectMembers where T : EntityManager
    {
        /// <summary>
        /// Configures the name of the <see cref="ConnectionOptions"/> to be used.
        /// </summary>
        /// <param name="connectionOptionsName">The name of the ConnectionOptions.</param>
        IEntityManagerProviderConfigurator<T> WithConnectionOptions(string connectionOptionsName);

        /// <summary>
        /// Configures the list of SampleDataProviders.
        /// </summary>
        /// <param name="sampleDataProviders">One or more sample data providers.</param>
        /// <remarks>If the SampleDataProviders are not configured, the EntityManagerProvider will try to discover them through the MEF container.</remarks>
        IEntityManagerProviderConfigurator<T> WithSampleDataProviders(params ISampleDataProvider<T>[] sampleDataProviders);

        /// <summary>
        /// Configures the SyncInterceptor.
        /// </summary>
        /// <param name="syncInterceptor">The SyncInterceptor to be used.</param>
        /// <remarks>If no SyncInterceptor is configured, the EntityManagerProvider will try to discover it through the MEF container.</remarks>
        IEntityManagerProviderConfigurator<T> WithSyncInterceptor(IEntityManagerSyncInterceptor syncInterceptor);

        /// <summary>
        /// Configures the EntityManagerDelegates.
        /// </summary>
        /// <param name="delegates">One or more EntityManagerDelegate.</param>
        /// <remarks>If the EntityManagerDelegates are not configured, the EntityManagerProvider will try to discover them through the MEF container.</remarks>
        IEntityManagerProviderConfigurator<T> WithDelegates(params EntityManagerDelegate<T>[] delegates);

        /// <summary>
        /// Configures the validation error notifiers.
        /// </summary>
        /// <param name="errorNotifiers">One or more validation error notifier.</param>
        /// <remarks>If the validation error notifiers are not configured, the EntityManagerProvider will try to discover them through the MEF container.</remarks>
        IEntityManagerProviderConfigurator<T> WithErrorNotifiers(params IValidationErrorNotification[] errorNotifiers);
    }

    /// <summary>
    ///   Manages and provides an EntityManager.
    /// </summary>
    /// <typeparam name="T"> The type of the EntityManager </typeparam>
    public partial class EntityManagerProvider<T> : IEntityManagerProvider<T>, IHandle<SyncDataMessage<T>>, IHandle<PrincipalChangedMessage>
        where T : EntityManager
    {
        private readonly EntityManagerProviderConfiguration<T> _configuration;
        private EntityManagerWrapper<T> _entityManagerWrapper;
        private IEnumerable<EntityKey> _deletedEntityKeys;
        private EntityCacheState _storeEcs;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        public EntityManagerProvider()
        {
            _configuration = new EntityManagerProviderConfiguration<T>();
        }

        /// <summary>
        /// Configures the current EntityManagerProvider.
        /// </summary>
        /// <param name="configure">Delegate to perform the configuration.</param>
        public EntityManagerProvider<T> Configure(Action<IEntityManagerProviderConfigurator<T>> configure)
        {
            configure(_configuration);
            return this;
        }

        #region IEntityManagerProvider<T> Members

        /// <summary>
        ///   Specifies the ConnectionOptions used by the current EntityManagerProvider.
        /// </summary>
        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.GetByName(_configuration.ConnectionOptionsName); }
        }

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
        public bool IsSaving { get; private set; }

        EntityManager IEntityManagerProvider.Manager
        {
            get { return Manager; }
        }

        /// <summary>
        ///   Returns the EntityManager managed by this provider.
        /// </summary>
        public T Manager
        {
            get
            {
                if (_entityManagerWrapper == null)
                {
                    _entityManagerWrapper = new EntityManagerWrapper<T>(CreateEntityManagerCore());
                    _entityManagerWrapper.EntityChanged +=
                        (sender, args) => OnEntityChanged(args.EntityAspect, args.Action);
                    _entityManagerWrapper.Querying += OnQuerying;
                    _entityManagerWrapper.Saving += OnSaving;
                    _entityManagerWrapper.Saved += OnSaved;

                    OnManagerCreated();
                }
                return _entityManagerWrapper.Manager;
            }
        }

        /// <summary>
        /// Signals that a Save of at least one entity has been performed
        /// or changed entities have been imported from another entity manager.
        /// Clients may use this event to force a data refresh. 
        /// </summary>
        public event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// Fired whenever an entity's state has changed in any significant manner.
        /// </summary>
        public event EventHandler<EntityChangedEventArgs> EntityChanged;

        /// <summary>
        /// Event fired after the EntityManager got created.
        /// </summary>
        public event EventHandler<EntityManagerCreatedEventArgs> ManagerCreated;

        #endregion

        #region IHandle<SyncDataMessage<T>> Members

        /// <summary>
        ///   Internal use.
        /// </summary>
        void IHandle<SyncDataMessage<T>>.Handle(SyncDataMessage<T> syncData)
        {
            if (syncData.IsSameProviderAs(this)) return;

            // Merge deletions
            var removers = syncData.DeletedEntityKeys
                .Select(key => Manager.FindEntity(key, false))
                .Where(entity => entity != null)
                .ToList();
            if (removers.Any()) Manager.RemoveEntities(removers, false);

            // Merge saved entities
            var mergers = syncData.SavedEntities.Where(SyncInterceptor.ShouldImportEntity);
            Manager.ImportEntities(mergers, MergeStrategy.PreserveChangesUpdateOriginal);

            // Signal to our clients that data has changed
            if (syncData.SavedEntities.Any() || syncData.DeletedEntityKeys.Any())
                RaiseDataChangedEvent(syncData.SavedEntities, syncData.DeletedEntityKeys);
        }

        #endregion

        internal OperationResult ResetFakeBackingStoreAsync()
        {
            EnsureSampleData();
            return FakeBackingStore.Get(CompositionContext.Name).ResetAsync(_storeEcs);
        }

        /// <summary>
        ///   Triggers the ManagerCreated event.
        /// </summary>
        protected virtual void OnManagerCreated()
        {
            if (ManagerCreated != null)
                ManagerCreated(this, new EntityManagerCreatedEventArgs(Manager));
        }

        /// <summary>
        ///   Triggers the DataChanged event.
        /// </summary>
        /// <param name="entityKeys"> The list of keys for the changed entities </param>
        protected virtual void OnDataChanged(IEnumerable<EntityKey> entityKeys)
        {
            if (DataChanged != null)
                DataChanged(this, new DataChangedEventArgs(entityKeys, Manager));
        }

        /// <summary>
        ///   Triggers the EntityChanged event.
        /// </summary>
        /// <param name="entityAspect">The changed entity's EntityAspect.</param>
        /// <param name="entityAction">The action that caused this change.</param>
        protected virtual void OnEntityChanged(EntityAspect entityAspect, EntityAction entityAction)
        {
            if (EntityChanged != null)
                EntityChanged(this, new EntityChangedEventArgs(entityAspect, entityAction));
        }

        /// <summary>
        ///   Creates a new EntityManager instance.
        /// </summary>
        protected virtual T CreateEntityManager()
        {
            try
            {
                var connectionOptions = ConnectionOptions;
                var manager = (T)Activator.CreateInstance(typeof(T), connectionOptions.ToEntityManagerContext());

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

        private T CreateEntityManagerCore()
        {
            if (Composition.IsRecomposing)
                throw new InvalidOperationException(StringResources.CreatingEntityManagerDuringRecompositionNotAllowed);

            Composition.BuildUp(this);
            EventFns.Subscribe(this);
            EnsureDelegates();
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

        private void PopulateStoreEcs()
        {
            // Create a separate isolated EntityManager
            var manager = CreateEntityManager();
            manager.Options.UseDefaultAuthenticationContext = false;
            manager.AuthenticationContext = AnonymousAuthenticationContext.Instance;
            manager.DefaultQueryStrategy = QueryStrategy.CacheOnly;
            manager.DefaultEntityReferenceStrategy = EntityReferenceStrategy.NoLoad;

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
                _entityManagerWrapper.Clear();
            }
            _entityManagerWrapper = null;
        }

        private IEntityManagerSyncInterceptor SyncInterceptor
        {
            get
            {
                if (_configuration.SyncInterceptor == null)
                {
                    var syncInterceptorLocator =
                        new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.NonShared, () => CompositionContext)
                            .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());
                    _configuration.WithSyncInterceptor(syncInterceptorLocator.GetPart());
                }

                // If custom implementation, set EntityManager property
                var syncInterceptor = _configuration.SyncInterceptor as EntityManagerSyncInterceptor;
                if (syncInterceptor != null)
                    syncInterceptor.EntityManager = Manager;

                return _configuration.SyncInterceptor;
            }
        }

        private void OnQuerying(object sender, EntityQueryingEventArgs args)
        {
            // In design mode all queries must be forced to execute against the cache.
            if (Execute.InDesignMode)
                args.Query = args.Query.With(QueryStrategy.CacheOnly);
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

                var syncEntities = e.Entities.Where(SyncInterceptor.ShouldExportEntity);
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

            foreach (var entity in args.Entities)
            {
                var entityAspect = EntityAspect.Wrap(entity);
                if (entityAspect.EntityState.IsDeletedOrDetached()) continue;

                var validationErrors = Manager.VerifierEngine.Execute(entity);
                foreach (var d in _configuration.Delegates ?? new EntityManagerDelegate<T>[0])
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
            _deletedEntityKeys = syncEntities
                .Where(e => EntityAspect.Wrap(e).EntityState.IsDeleted())
                .Select(e => EntityAspect.Wrap(e).EntityKey)
                .ToList();
        }

        private void OnSaved(object sender, EntitySavedEventArgs e)
        {
            try
            {
                if (!e.HasError)
                {
                    var exportEntities = e.Entities
                        .Where(entity => SyncInterceptor.ShouldExportEntity(entity) &&
                                         !_deletedEntityKeys
                                              .Contains(EntityAspect.Wrap(entity).EntityKey))
                        .ToList();
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

        private void RaiseDataChangedEvent(IEnumerable<object> savedEntities, IEnumerable<EntityKey> deletedEntityKeys)
        {
            var entityKeys = savedEntities
                .Select(e => EntityAspect.Wrap(e).EntityKey)
                .Concat(deletedEntityKeys)
                .ToList();

            if (!entityKeys.Any()) return;

            OnDataChanged(entityKeys);
        }

        private CompositionContext CompositionContext
        {
            get { return ConnectionOptions.CompositionContext; }
        }

        private void EnsureDelegates()
        {
            if (_configuration.Delegates != null) return;

            var i = CompositionContext.GetExportedInstances(typeof(EntityManagerDelegate));
            if (i != null)
                _configuration.WithDelegates(i.OfType<EntityManagerDelegate<T>>().ToArray());

            if (_configuration.Delegates == null || !_configuration.Delegates.Any())
                _configuration.WithDelegates(Composition.GetInstances<EntityManagerDelegate>()
                                                 .OfType<EntityManagerDelegate<T>>()
                                                 .ToArray());

            TraceFns.WriteLine(_configuration.Delegates.Any()
                                   ? string.Format(StringResources.ProbedForEntityManagerDelegateAndFoundMatch,
                                                   _configuration.Delegates.Count())
                                   : StringResources.ProbedForEntityManagerDelegateAndFoundNoMatch);
        }

        private IEnumerable<IValidationErrorNotification> ValidationErrorNotifiers
        {
            get
            {
                if (_configuration.ErrorNotifiers == null)
                {
                    var i = CompositionContext.GetExportedInstances(typeof(IValidationErrorNotification));
                    if (i != null)
                        _configuration.WithErrorNotifiers(i.Cast<IValidationErrorNotification>().ToArray());

                    if (_configuration.ErrorNotifiers == null || !_configuration.ErrorNotifiers.Any())
                        _configuration.WithErrorNotifiers(Composition.GetInstances<IValidationErrorNotification>().ToArray());

                    TraceFns.WriteLine(_configuration.ErrorNotifiers.Any()
                                           ? string.Format(
                                               StringResources.ProbedForIValidationErrorNotificationAndFoundMatch,
                                               _configuration.ErrorNotifiers.Count())
                                           : StringResources.ProbedForIValidationErrorNotificationAndFoundNoMatch);
                }

                return _configuration.ErrorNotifiers;
            }
        }

        private IEnumerable<ISampleDataProvider<T>> SampleDataProviders
        {
            get
            {
                if (_configuration.SampleDataProviders == null)
                    _configuration.WithSampleDataProviders(Composition.GetInstances<ISampleDataProvider<T>>().ToArray());

                return _configuration.SampleDataProviders;
            }
        }

        private void EnsureSampleData()
        {
            if (!FakeBackingStore.Exists(CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            if (_storeEcs != null)
                return;

            PopulateStoreEcs();
        }
    }

    internal class EntityManagerProviderConfiguration<T> : IEntityManagerProviderConfigurator<T> where T : EntityManager
    {
        public IEntityManagerProviderConfigurator<T> WithConnectionOptions(string connectionOptionsName)
        {
            ConnectionOptionsName = connectionOptionsName;
            return this;
        }

        public IEntityManagerProviderConfigurator<T> WithSampleDataProviders(params ISampleDataProvider<T>[] sampleDataProviders)
        {
            SampleDataProviders = sampleDataProviders;
            return this;
        }

        public IEntityManagerProviderConfigurator<T> WithSyncInterceptor(IEntityManagerSyncInterceptor syncInterceptor)
        {
            SyncInterceptor = syncInterceptor;
            return this;
        }

        public IEntityManagerProviderConfigurator<T> WithDelegates(params EntityManagerDelegate<T>[] delegates)
        {
            Delegates = delegates;
            return this;
        }

        public IEntityManagerProviderConfigurator<T> WithErrorNotifiers(params IValidationErrorNotification[] errorNotifiers)
        {
            ErrorNotifiers = errorNotifiers;
            return this;
        }

        public string ConnectionOptionsName { get; private set; }

        public IEnumerable<ISampleDataProvider<T>> SampleDataProviders { get; private set; }

        public IEntityManagerSyncInterceptor SyncInterceptor { get; private set; }

        public IEnumerable<EntityManagerDelegate<T>> Delegates { get; private set; }

        public IEnumerable<IValidationErrorNotification> ErrorNotifiers { get; private set; }
    }
}