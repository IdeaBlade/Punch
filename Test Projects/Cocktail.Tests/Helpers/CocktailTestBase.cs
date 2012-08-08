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

using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests.Helpers
{
    public class CocktailTestBase
    {
        static CocktailTestBase()
        {
            EntityManager.EntityManagerCreated += OnEntityManagerCreated;
        }

        private static void OnEntityManagerCreated(object sender, EntityManagerCreatedEventArgs e)
        {
            // Keep each EM's authentication context separate for testing.
            e.EntityManager.Options.UseDefaultAuthenticationContext = false;
        }

        /// <summary>
        /// Called before each test
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            Composition.Clear();
            Authenticator.Instance.DefaultAuthenticationContext = null;
            var batch = new CompositionBatch();
            PrepareCompositionContainer(batch);
            Composition.Configure(batch);
        }

        protected virtual void PrepareCompositionContainer(CompositionBatch batch)
        {
        }

        public async Task ResetFakeBackingStoreAsync(string compositionContextName)
        {
            var provider = EntityManagerProviderFactory
                .CreateTestEntityManagerProvider(compositionContextName);
            if (provider != null)
            {
#if SILVERLIGHT
                await provider.ResetFakeBackingStoreAsync();
#else
                provider.ResetFakeBackingStore();
                await Task.FromResult(true);
#endif
            }
        }

        public async Task TestInit(string compositionContextName)
        {
            var provider = EntityManagerProviderFactory
                .CreateTestEntityManagerProvider(compositionContextName);
            if (provider != null)
            {
#if SILVERLIGHT
                await provider.InitializeFakeBackingStoreAsync();
#else
                provider.InitializeFakeBackingStore();
                await Task.FromResult(true);
#endif
            }
            await ResetFakeBackingStoreAsync(compositionContextName);
        }
    }
}