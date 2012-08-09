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

using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public static partial class FakeStoreEntityManagerProviderFns
    {
        /// <summary>
        ///   Initializes the fake backing store.
        /// </summary>
        /// <returns> Returns true if the EntityManagerProvider supports the fake backing store. </returns>
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

        /// <summary>
        ///   Resets the fake backing store to its initial state.
        /// </summary>
        /// <returns> Returns true if the EntityManagerProvider supports the fake backing store. </returns>
        public static bool ResetFakeBackingStore<T>(this IEntityManagerProvider<T> @this) where T : EntityManager
        {
            if (!@this.ConnectionOptions.IsFake || !(@this is EntityManagerProvider<T>))
            {
                DebugFns.WriteLine(StringResources.NonSuitableEmpForFakeStoreOperation);
                return false;
            }

            ((EntityManagerProvider<T>) @this).ResetFakeBackingStore();
            return true;
        }
    }
}