using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Provides extension methods to initialize and reset the DevForce Fake Backing Store
    /// </summary>
    public static class FakeStoreEntityManagerProviderFns
    {
        /// <summary>Initializes the fake backing store.</summary>
        public static OperationResult InitializeFakeBackingStoreAsync<T>(this IEntityManagerProvider<T> @this)
            where T : EntityManager
        {
            if (!@this.ConnectionOptions.IsFake)
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return AlwaysCompletedOperationResult.Instance;
            }

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
            if (!@this.ConnectionOptions.IsFake && @this is EntityManagerProvider<T>)
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return AlwaysCompletedOperationResult.Instance;
            }

            return ((EntityManagerProvider<T>)@this).ResetFakeBackingStoreAsync();
        }

#if !SILVERLIGHT

        /// <summary>Initializes the fake backing store.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static bool InitializeFakeBackingStore<T>(this IEntityManagerProvider<T> @this) where T : EntityManager
        {
            if (!@this.ConnectionOptions.IsFake)
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return false;
            }

            // Return if already initialized
            if (FakeBackingStore.Exists(@this.Manager.CompositionContext.Name))
                return true;

            FakeBackingStore.Create(@this.Manager.CompositionContext.Name);

            ResetFakeBackingStore(@this);
            return true;
        }

        /// <summary>Resets the fake backing store to its initial state.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static bool ResetFakeBackingStore<T>(this IEntityManagerProvider<T> @this) where T : EntityManager
        {
            if (!@this.ConnectionOptions.IsFake && @this is EntityManagerProvider<T>)
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return false;
            }

            ((EntityManagerProvider<T>)@this).ResetFakeBackingStore();
            return true;
        }

#endif
    }
}