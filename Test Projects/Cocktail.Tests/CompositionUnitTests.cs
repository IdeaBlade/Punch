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
using System.ComponentModel.Composition.Hosting;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.TestFramework;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class CompositionUnitTests : DevForceTest
    {
        public CompositionUnitTests()
        {
            Composition.Configure();

            var batch = new CompositionBatch();
            NorthwindIBEntities entityManager = EntityManagerProviderFactory.CreateTestEntityManagerProvider().Manager;
            batch.AddExportedValue<IAuthenticationService>(new AuthenticationService<NorthwindIBEntities>(entityManager));
            Composition.Compose(batch);
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldDiscoverInjectedAuthenticationService()
        {
            var injectedService = new AuthenticationService<NorthwindIBEntities>(new NorthwindIBEntities(false));

            var ctx = CompositionContext.Default
                .WithGenerator(typeof(IAuthenticationService), () => injectedService)
                .WithName("Test");

            var auth1 = new PartLocator<IAuthenticationService>(CreationPolicy.Any, () => ctx).GetPart();
            var auth2 = new PartLocator<IAuthenticationService>(CreationPolicy.Any).GetPart();

            Assert.IsNotNull(auth1, "AuthenticationServer should not be null");
            Assert.IsNotNull(auth2, "AuthenticationService should not be null");
            Assert.IsTrue(ReferenceEquals(auth1, injectedService), "Should have found service in CompositionContext");
            Assert.IsFalse(ReferenceEquals(auth1, auth2), "Should have discovered different authentication services");
        }

        //[TestMethod]
        //[Tag("Composition")]
        //public void ShouldComposeUsingChildContainer()
        //{
        //    var context1 = new CompositionContext();
        //    context1.AddInstance<IInjectedObject>(new InjectedObjectA());

        //    var context2 = new CompositionContext();
        //    context2.AddInstance<IInjectedObject>(new InjectedObjectB());


        //    var obj1 = context1.GetInstance<ChildContainerTestObject>();
        //    Assert.IsNotNull(obj1.InjectedObject.GetType() == typeof(InjectedObjectA),
        //                     "Should have composed InjectedObjectA");

        //    var obj2 = context2.GetInstance<ChildContainerTestObject>();
        //    Assert.IsNotNull(obj2.InjectedObject.GetType() != typeof(InjectedObjectB),
        //                     "Should have composed InjectedObjectB");
        //}

        [TestMethod]
        [Tag("Composition")]
        public void ShouldDiscoverDefault()
        {
            var context = CompositionContext.Default
                .WithGenerator(typeof(IEntityManagerSyncInterceptor), () => new SyncInterceptor())
                .WithName("Test");

            var partLocator1 = new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.Any, () => context);
            var partLocator2 = new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.Any)
                .WithDefaultGenerator(() => new DefaultEntityManagerSyncInterceptor());

            var obj1 = partLocator1.GetPart();
            Assert.IsTrue(obj1.GetType() == typeof(SyncInterceptor),
                             "Should have found the SyncInterceptor");

            var obj2 = partLocator2.GetPart();
            Assert.IsTrue(obj2.GetType() == typeof(DefaultEntityManagerSyncInterceptor),
                             "Should have found the DefaultSyncInterceptor");
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldReturnSharedInstance()
        {
            var instance1 = Composition.GetInstance<SharedObject>();
            var instance2 = Composition.GetInstance<SharedObject>();

            Assert.IsTrue(ReferenceEquals(instance1, instance2), "Should have the same instance");
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldReturnNonSharedInstance()
        {
            var instance1 = Composition.GetInstance<NonSharedObject>();
            var instance2 = Composition.GetInstance<NonSharedObject>();

            Assert.IsFalse(ReferenceEquals(instance1, instance2), "Should have two seperate instances");
        }

        [TestMethod]
        [Tag("Composition")]
        [Asynchronous, Timeout(10000)]
        public void ShouldRaiseQueryEvents()
        {
            var interceptor = new TestEntityManagerDelegate();
            var contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ContextWithEntityManagerDelegate");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ContextWithEntityManagerDelegate");

            DoItAsync(
                () =>
                {
                    Assert.IsTrue(interceptor.QueriedRaised == 0);
                    Assert.IsTrue(interceptor.QueryingRaised == 0);
                    Assert.IsTrue(interceptor.FetchingRaised == 0);

                    EntityQuery<Customer> q = emp.Manager.Customers;
                    var asyncFns = new Func<INotifyCompleted>[]
                                           {
                                               emp.InitializeAsync,
                                               () => q.ExecuteAsync(op =>
                                                                        {
                                                                            Assert.IsTrue(interceptor.QueriedRaised > 0);
                                                                            Assert.IsTrue(interceptor.QueryingRaised > 0);
                                                                            Assert.IsTrue(interceptor.FetchingRaised > 0);

                                                                            Assert.IsTrue(interceptor.QueryingRaised < interceptor.FetchingRaised);
                                                                            Assert.IsTrue(interceptor.FetchingRaised < interceptor.QueriedRaised);
                                                                        })
                                           };

                    Coroutine.Start(asyncFns, op => TestComplete());
                });
        }

        [TestMethod]
        [Tag("Composition")]
        [Asynchronous, Timeout(10000)]
        public void ShouldRaiseSaveEvents()
        {
            var interceptor = new TestEntityManagerDelegate();
            var contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ContextWithEntityManagerDelegate");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ContextWithEntityManagerDelegate");

            DoItAsync(
                () =>
                {
                    Assert.IsTrue(interceptor.SavingRaised == 0);
                    Assert.IsTrue(interceptor.SavedRaised == 0);
                    Assert.IsTrue(interceptor.EntityChangingRaised == 0);
                    Assert.IsTrue(interceptor.EntityChangedRaised == 0);

                    var customer = emp.Manager.CreateEntity<Customer>();
                    emp.Manager.AddEntity(customer);
                    customer.CompanyName = "Foo";

                    var asyncFns = new Func<INotifyCompleted>[]
                                           {
                                               emp.InitializeAsync,
                                               () => emp.Manager.SaveChangesAsync(
                                                   op =>
                                                       {
                                                           Assert.IsTrue(interceptor.SavingRaised > 0);
                                                           Assert.IsTrue(interceptor.SavedRaised > 0);
                                                           Assert.IsTrue(interceptor.EntityChangingRaised > 0);
                                                           Assert.IsTrue(interceptor.EntityChangedRaised > 0);

                                                           Assert.IsTrue(interceptor.EntityChangingRaised < interceptor.EntityChangedRaised);
                                                           Assert.IsTrue(interceptor.SavingRaised < interceptor.SavedRaised);
                                                           Assert.IsTrue(interceptor.SavingRaised < interceptor.EntityChangingRaised);
                                                           Assert.IsTrue(interceptor.EntityChangedRaised < interceptor.SavedRaised);
                                                       })
                                           };

                    Coroutine.Start(asyncFns, op => TestComplete());
                });
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldRaiseClearedEvent()
        {
            var interceptor = new TestEntityManagerDelegate();
            var contextWithEntityManagerDelegate = CompositionContext.Fake
                .WithGenerator(typeof(EntityManagerDelegate), () => interceptor)
                .WithName("ContextWithEntityManagerDelegate");
            CompositionContextResolver.Add(contextWithEntityManagerDelegate);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider("ContextWithEntityManagerDelegate");

            Assert.IsTrue(interceptor.ClearedRaised == 0);

            emp.Manager.Clear();

            Assert.IsTrue(interceptor.ClearedRaised > 0);
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldInstantiateMultipleBootstrappers()
        {
            try
            {
                var b1 = new FrameworkBootstrapper<object>();
                var b2 = new FrameworkBootstrapper<string>();
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(false, "Should not have thrown InvalidOperationException");
                return;
            }
        }
    }
}