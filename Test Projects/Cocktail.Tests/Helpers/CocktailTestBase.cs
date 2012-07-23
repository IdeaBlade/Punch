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
using System.ComponentModel.Composition.Hosting;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using IdeaBlade.TestFramework;

namespace Cocktail.Tests.Helpers
{
    public class CocktailTestBase : DevForceTest
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
        protected override void Context()
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

        public INotifyCompleted ResetFakeBackingStore(string compositionContextName)
        {
            var provider =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(compositionContextName);
            if (provider != null)
                return provider.ResetFakeBackingStoreAsync();

            return AlwaysCompleted.Instance;
        }

        public INotifyCompleted TestInit(string compositionContextName)
        {
            var commands = new List<Func<INotifyCompleted>>
                               {
                                   () => EntityManagerProviderFactory.CreateTestEntityManagerProvider(compositionContextName)
                                       .InitializeFakeBackingStoreAsync(),
                                   () => ResetFakeBackingStore(compositionContextName)
                               };
            return Coroutine.Start(commands);
        }
    }
}