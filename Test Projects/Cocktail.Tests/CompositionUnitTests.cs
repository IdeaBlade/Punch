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
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class CompositionUnitTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldDiscoverInjectedAuthenticationService()
        {
            var injectedService = new AuthenticationService();

            CompositionContext ctx = CompositionContext.Default
                .WithGenerator(typeof(IAuthenticationService), () => injectedService)
                .WithName("ShouldDiscoverInjectedAuthenticationService");

            IAuthenticationService auth1 =
                new PartLocator<IAuthenticationService>(CreationPolicy.Any, () => ctx).GetPart();
            IAuthenticationService auth2 = new PartLocator<IAuthenticationService>(CreationPolicy.Any).GetPart();

            Assert.IsNotNull(auth1, "AuthenticationServer should not be null");
            Assert.IsNull(auth2, "AuthenticationService should be null");
            Assert.IsTrue(ReferenceEquals(auth1, injectedService), "Should have found service in CompositionContext");
        }

        [TestMethod]
        public void ShouldDiscoverDefault()
        {
            CompositionContext context = CompositionContext.Default
                .WithGenerator(typeof(IEntityManagerSyncInterceptor), () => new SyncInterceptor())
                .WithName("ShouldDiscoverDefault");

            var partLocator1 = new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.Any, () => context);
            PartLocator<IEntityManagerSyncInterceptor> partLocator2 = new PartLocator<IEntityManagerSyncInterceptor>(
                CreationPolicy.Any)
                .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());

            IEntityManagerSyncInterceptor obj1 = partLocator1.GetPart();
            Assert.IsTrue(obj1.GetType() == typeof(SyncInterceptor),
                          "Should have found the SyncInterceptor");

            IEntityManagerSyncInterceptor obj2 = partLocator2.GetPart();
            Assert.IsTrue(obj2.GetType() == typeof(DefaultEntityManagerSyncInterceptor),
                          "Should have found the DefaultSyncInterceptor");
        }

        [TestMethod]
        public void ShouldReturnSharedInstance()
        {
            var instance1 = Composition.GetInstance<SharedObject>();
            var instance2 = Composition.GetInstance<SharedObject>();

            Assert.IsTrue(ReferenceEquals(instance1, instance2), "Should have the same instance");
        }

        [TestMethod]
        public void ShouldReturnNonSharedInstance()
        {
            var instance1 = Composition.GetInstance<NonSharedObject>();
            var instance2 = Composition.GetInstance<NonSharedObject>();

            Assert.IsFalse(ReferenceEquals(instance1, instance2), "Should have two separate instances");
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRaiseQueryEvents()
        {
            var interceptor = new TestEntityManagerDelegate();
            CompositionContext contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ShouldRaiseQueryEvents");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ShouldRaiseQueryEvents");

            Assert.IsTrue(interceptor.QueriedRaised == 0);
            Assert.IsTrue(interceptor.QueryingRaised == 0);
            Assert.IsTrue(interceptor.FetchingRaised == 0);

            await InitFakeBackingStoreAsync(emp.ConnectionOptions.CompositionContext.Name);
            await emp.Manager.Customers.ExecuteAsync();

            Assert.IsTrue(interceptor.QueriedRaised > 0);
            Assert.IsTrue(interceptor.QueryingRaised > 0);
            Assert.IsTrue(interceptor.FetchingRaised > 0);

            Assert.IsTrue(interceptor.QueryingRaised < interceptor.FetchingRaised);
            Assert.IsTrue(interceptor.FetchingRaised < interceptor.QueriedRaised);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRaiseSaveEvents()
        {
            var interceptor = new TestEntityManagerDelegate();
            CompositionContext contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ShouldRaiseSaveEvents");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ShouldRaiseSaveEvents");

            int entityChangedRaised = 0;
            emp.EntityChanged += (sender, args) => entityChangedRaised++;

            Assert.IsTrue(interceptor.SavingRaised == 0);
            Assert.IsTrue(interceptor.SavedRaised == 0);
            Assert.IsTrue(interceptor.EntityChangingRaised == 0);
            Assert.IsTrue(interceptor.EntityChangedRaised == 0);

            var customer = emp.Manager.CreateEntity<Customer>();
            emp.Manager.AddEntity(customer);
            customer.CompanyName = "Foo";

            await InitFakeBackingStoreAsync(emp.Manager.CompositionContext.Name);
            await emp.Manager.SaveChangesAsync();

            Assert.IsTrue(entityChangedRaised == 3);
            Assert.IsTrue(interceptor.SavingRaised > 0);
            Assert.IsTrue(interceptor.SavedRaised > 0);
            Assert.IsTrue(interceptor.EntityChangingRaised > 0);
            Assert.IsTrue(interceptor.EntityChangedRaised > 0);

            Assert.IsTrue(interceptor.EntityChangingRaised < interceptor.EntityChangedRaised);
            Assert.IsTrue(interceptor.SavingRaised < interceptor.SavedRaised);
            Assert.IsTrue(interceptor.SavingRaised < interceptor.EntityChangingRaised);
            Assert.IsTrue(interceptor.EntityChangedRaised < interceptor.SavedRaised);
        }

        [TestMethod]
        public void ShouldRaiseClearedEvent()
        {
            var interceptor = new TestEntityManagerDelegate();
            CompositionContext contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ShouldRaiseClearedEvent");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ShouldRaiseClearedEvent");

            Assert.IsTrue(interceptor.ClearedRaised == 0);

            emp.Manager.Clear();

            Assert.IsTrue(interceptor.ClearedRaised > 0);
        }

        [TestMethod]
        public void ObjectManagerShouldCreateObject()
        {
            var objectManager = new ObjectManager<Guid, ICustomerRepository>();

            var key = Guid.NewGuid();
            var obj = objectManager.GetObject(key);
            Assert.IsNotNull(obj);

            obj = objectManager.TryGetObject(key);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void ObjectManagerShouldReturnNullIfObjectDoesntExist()
        {
            var objectManager = new ObjectManager<Guid, ICustomerRepository>();
            var obj = objectManager.TryGetObject(Guid.NewGuid());
            Assert.IsNull(obj);
        }
    }
}