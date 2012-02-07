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
    public abstract class FakeStoreEntityManagerProviderBase<T> : EntityManagerProviderBase<T>
        where T : EntityManager
    {
        /// <summary>Internal use.</summary>
        [ImportMany]
        public IEnumerable<ISampleDataProvider<T>> SampleDataProviders { get; set; }

        /// <summary>Holds the serialized sample data used to initialize and reset the fake backing store.</summary>
        protected static EntityCacheState StoreEcs { get; private set; }

        internal OperationResult ResetFakeBackingStoreAsync()
        {
            if (!FakeBackingStore.Exists(Manager.CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            // Create a separate isolated EntityManager
            T manager = CreateEntityManager();
            LinkAuthentication(manager);

            if (StoreEcs == null)
                PopulateStoreEcs(manager);

            return FakeBackingStore.Get(Manager.CompositionContext.Name).ResetAsync(manager, StoreEcs);
        }

#if !SILVERLIGHT

        internal void ResetFakeBackingStore()
        {
            if (!FakeBackingStore.Exists(Manager.CompositionContext.Name))
                throw new InvalidOperationException(StringResources.TheFakeStoreHasNotBeenInitialized);

            // Create a separate isolated EntityManager
            T manager = CreateEntityManager();
            LinkAuthentication(manager);

            if (StoreEcs == null)
                PopulateStoreEcs(manager);

            FakeBackingStore.Get(Manager.CompositionContext.Name).Reset(manager, StoreEcs);
        }

#endif

        private void PopulateStoreEcs(T manager)
        {
            if (SampleDataProviders != null)
                SampleDataProviders.ForEach(p => p.AddSampleData(manager));

            StoreEcs = manager.CacheStateManager.GetCacheState();
            // We used the manager just to get the ECS; now clear it out
            manager.Clear();
        }
    }

    /// <summary>
    /// Provides extension methods to initialize and reset the DevForce Fake Backing Store
    /// </summary>
    public static class FakeStoreEntityManagerProviderFns
    {
        /// <summary>Initializes the fake backing store.</summary>
        public static OperationResult InitializeFakeBackingStoreAsync<T>(this IEntityManagerProvider<T> @this)
            where T : EntityManager
        {
            if (!(@this is FakeStoreEntityManagerProviderBase<T>))
                return AlwaysCompletedOperationResult.Instance;

            string compositionContext = @this.Manager.CompositionContext.Name;
            // Return the operation if fake store object already exists.
            if (FakeBackingStore.Exists(compositionContext))
                return FakeBackingStore.Get(compositionContext).InitializeOperation.AsOperationResult();

            FakeBackingStore.Create(compositionContext);

            return ResetFakeBackingStoreAsync(@this);
        }

        /// <summary>Resets the fake backing store to its initial state.</summary>
        public static OperationResult ResetFakeBackingStoreAsync<T>(this IEntityManagerProvider<T> @this)
            where T : EntityManager
        {
            if (!(@this is FakeStoreEntityManagerProviderBase<T>))
                return AlwaysCompletedOperationResult.Instance;

            var entityManagerProvider = (FakeStoreEntityManagerProviderBase<T>) @this;
            return entityManagerProvider.ResetFakeBackingStoreAsync();
        }

#if !SILVERLIGHT

        /// <summary>Initializes the fake backing store.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static bool InitializeFakeBackingStore<T>(this IEntityManagerProvider<T> @this) where T : EntityManager
        {
            if (!(@this is FakeStoreEntityManagerProviderBase<T>)) return false;

            FakeBackingStore.Create(@this.Manager.CompositionContext.Name);

            ResetFakeBackingStore(@this);
            return true;
        }

        /// <summary>Resets the fake backing store to its initial state.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static bool ResetFakeBackingStore<T>(this IEntityManagerProvider<T> @this) where T : EntityManager
        {
            if (!(@this is FakeStoreEntityManagerProviderBase<T>)) return false;

            var entityManagerProvider = (FakeStoreEntityManagerProviderBase<T>) @this;
            entityManagerProvider.ResetFakeBackingStore();
            return true;
        }

#endif
    }
}