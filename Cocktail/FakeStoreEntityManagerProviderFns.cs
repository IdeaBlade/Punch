using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Provides extension methods to initialize and reset the DevForce Fake Backing Store
    /// </summary>
    public static partial class FakeStoreEntityManagerProviderFns
    {
        /// <summary>Initializes the fake backing store.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static async Task<bool> InitializeFakeBackingStoreAsync<T>(this IEntityManagerProvider<T> @this)
            where T : EntityManager
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

            await ResetFakeBackingStoreAsync(@this);
            return true;
        }

        /// <summary>Resets the fake backing store to its initial state.</summary>
        /// <returns>Returns true if the EntityManagerProvider supports the fake backing store.</returns>
        public static async Task<bool> ResetFakeBackingStoreAsync<T>(this IEntityManagerProvider<T> @this)
            where T : EntityManager
        {
            if (!@this.ConnectionOptions.IsFake || !(@this is EntityManagerProvider<T>))
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return false;
            }

            await ((EntityManagerProvider<T>)@this).ResetFakeBackingStoreAsync();
            return true;
        }
    }
}