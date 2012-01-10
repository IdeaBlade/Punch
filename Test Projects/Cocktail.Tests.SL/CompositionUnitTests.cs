//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Cocktail.Tests.Helpers;
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
            CompositionHelper.Configure();

            var batch = new CompositionBatch();
            NorthwindIBEntities entityManager = EntityManagerProviderFactory.CreateTestEntityManagerProvider().Manager;
            batch.AddExportedValue<IAuthenticationService>(new AuthenticationService<NorthwindIBEntities>(entityManager));
            CompositionHelper.Compose(batch);
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldAlwaysDiscoverSameAuthenticationService()
        {
            var ctx1 = new CompositionContext();
            var ctx2 = new CompositionContext();

            var auth1 = ctx1.GetInstance<IAuthenticationService>();
            var auth2 = ctx2.GetInstance<IAuthenticationService>();

            Assert.IsNotNull(auth1, "AuthenticationServer should not be null");
            Assert.IsNotNull(auth2, "AuthenticationService should not be null");
            Assert.IsTrue(ReferenceEquals(auth1, auth2), "Should have discovered same authentication service");
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldComposeUsingChildContainer()
        {
            var context1 = new CompositionContext();
            context1.AddInstance<IInjectedObject>(new InjectedObjectA());

            var context2 = new CompositionContext();
            context2.AddInstance<IInjectedObject>(new InjectedObjectB());


            var obj1 = context1.GetInstance<ChildContainerTestObject>();
            Assert.IsNotNull(obj1.InjectedObject.GetType() == typeof(InjectedObjectA),
                             "Should have composed InjectedObjectA");

            var obj2 = context2.GetInstance<ChildContainerTestObject>();
            Assert.IsNotNull(obj2.InjectedObject.GetType() != typeof(InjectedObjectB),
                             "Should have composed InjectedObjectB");
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldIsolateChildContainers()
        {
            var context1 = new CompositionContext();
            context1.AddInstance<IEntityManagerSyncInterceptor>(new SyncInterceptor());
            var context2 = new CompositionContext();

            var partLocator1 = new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.Any, true, context1);
            var partLocator2 = new PartLocator<IEntityManagerSyncInterceptor>(CreationPolicy.Any, true, context2)
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
            var instance1 = CompositionHelper.GetInstance<SharedObject>();
            var instance2 = CompositionHelper.GetInstance<SharedObject>();

            Assert.IsTrue(ReferenceEquals(instance1, instance2), "Should have the same instance");
        }

        [TestMethod]
        [Tag("Composition")]
        public void ShouldReturnNonSharedInstance()
        {
            var instance1 = CompositionHelper.GetInstance<NonSharedObject>();
            var instance2 = CompositionHelper.GetInstance<NonSharedObject>();

            Assert.IsFalse(ReferenceEquals(instance1, instance2), "Should have two seperate instances");
        }

        [TestMethod]
        [Tag("Composition")]
        [Asynchronous, Timeout(10000)]
        public void ShouldRaiseQueryEvents()
        {
            var interceptor = new TestEntityManagerInterceptor();
            var ctx = new CompositionContext();
            ctx.AddInstance<EntityManagerInterceptor>(interceptor);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(ctx);

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
            var interceptor = new TestEntityManagerInterceptor();
            var ctx = new CompositionContext();
            ctx.AddInstance<EntityManagerInterceptor>(interceptor);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(ctx);

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
            var interceptor = new TestEntityManagerInterceptor();
            var ctx = new CompositionContext();
            ctx.AddInstance<EntityManagerInterceptor>(interceptor);

            IEntityManagerProvider<NorthwindIBEntities> emp =
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(ctx);

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