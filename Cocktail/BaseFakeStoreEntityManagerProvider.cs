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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Base class for an EntityManagerProvider with a fake backing store.
    /// Extend from this class to implement a Development EntityManagerProvider
    /// </summary>
    /// <typeparam name="T">The type of the EntityManager</typeparam>
    public abstract class BaseFakeStoreEntityManagerProvider<T> : BaseEntityManagerProvider<T>
        where T : EntityManager
    {
        /// <summary>Initializes a new instance.</summary>
        /// <param name="compositionContext">The CompositionContext to be used. If not provided a new default context will be created.</param>
        /// <param name="sampleDataProviders">An optional collection of sample data providers. If not provided, the framework will attempt to discover sample data providers.</param>
        /// <param name="authenticationService">The authentication service to be used. If not provided, the framework will attempt to discover the current authentication service.</param>
        protected BaseFakeStoreEntityManagerProvider(IAuthenticationService authenticationService = null,
                                                     CompositionContext compositionContext = null,
                                                     params ISampleDataProvider<T>[] sampleDataProviders)
            : base(authenticationService, compositionContext)
        {
            SampleDataProviders = sampleDataProviders;
        }

        /// <summary>Internal use.</summary>
        [ImportMany]
        public IEnumerable<ISampleDataProvider<T>> SampleDataProviders { get; set; }

        /// <summary>Holds the serialized sample data used to initialize and reset the fake backing store.</summary>
        protected static EntityCacheState StoreEcs { get; private set; }

        /// <summary>Indicates whether the fake backing store has been initialized.</summary>
        public override bool IsInitialized
        {
            get
            {
                return FakeBackingStore.Exists(Manager.CompositionContext.Name) &&
                       FakeBackingStore.Get(Manager.CompositionContext.Name).IsInitialized;
            }
        }

        /// <summary>Initializes the fake backing store.</summary>
        public override INotifyCompleted InitializeAsync()
        {
            // Return the operation if fake store object already exists.
            if (FakeBackingStore.Exists(Manager.CompositionContext.Name))
                return FakeBackingStore.Get(Manager.CompositionContext.Name).InitializeOperation;

            FakeBackingStore.Create(Manager.CompositionContext.Name);

            return ResetFakeBackingStoreAsync();
        }

#if SILVERLIGHT
        /// <summary>Resets the fake backing store to its initial state.</summary>
        [Obsolete("Use ResetFakeBackingStoreAsync instead")]
        public INotifyCompleted ResetFakeBackingStore()
        {
            return ResetFakeBackingStoreAsync();
        }
#endif

        /// <summary>Resets the fake backing store to its initial state.</summary>
        public INotifyCompleted ResetFakeBackingStoreAsync()
        {
            if (!FakeBackingStore.Exists(Manager.CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheEntityManagerProviderHasNotBeenInitialized);

            // Create a seperate isolated EntityManager
            var manager = CreateEntityManager();
            LinkAuthentication(manager);

            if (StoreEcs == null)
                PopulateStoreEcs(manager);

            return FakeBackingStore.Get(Manager.CompositionContext.Name).ResetAsync(manager, StoreEcs);
        }

#if !SILVERLIGHT

        /// <summary>Initializes the fake backing store.</summary>
        public override void Initialize()
        {
            FakeBackingStore.Create(Manager.CompositionContext.Name);

            ResetFakeBackingStore();
        }

        /// <summary>Resets the fake backing store to its initial state.</summary>
        public void ResetFakeBackingStore()
        {
            if (!FakeBackingStore.Exists(Manager.CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheEntityManagerProviderHasNotBeenInitialized);

            // Create a seperate isolated EntityManager
            var manager = CreateEntityManager();
            LinkAuthentication(manager);

            if (StoreEcs == null)
                PopulateStoreEcs(manager);

            FakeBackingStore.Get(Manager.CompositionContext.Name).Reset(manager, StoreEcs);
        }

#endif

        internal void PopulateStoreEcs(T manager)
        {
            if (SampleDataProviders != null)
                SampleDataProviders.ForEach(p => p.AddSampleData(manager));

            StoreEcs = manager.CacheStateManager.GetCacheState();
            // We used the manager just to get the ECS; now clear it out
            manager.Clear();
        }
    }
}