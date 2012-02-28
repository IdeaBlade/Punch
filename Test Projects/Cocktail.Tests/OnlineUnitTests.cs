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
using System.Linq;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class OnlineUnitTests : CocktailTestBase
    {
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

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        [Tag("Online")]
        public void ShouldGetCustomers()
        {
            var repository = new CustomerRepository(EntityManagerProviderFactory.CreateTestEntityManagerProvider());

            DoItAsync(
                () =>
                {
                    var commands = new List<Func<INotifyCompleted>>
                                           {
                                               () => TestInit(CompositionContext.Fake.Name),
                                               () =>
                                               repository.GetCustomers(null,
                                                                       customers =>
                                                                           {
                                                                               Assert.IsTrue(customers.Any(),
                                                                                             "Should have some customers");
                                                                               TestComplete();
                                                                           })
                                           };

                    Coroutine.Start(commands);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        [Tag("Online")]
        public void ShouldSynchronizeDeletesBetweenEntityManagers()
        {
            CompositionContext compositionContextWithSyncInterceptor = CompositionContext.Fake
                .WithGenerator(typeof(IEntityManagerSyncInterceptor), () => new SyncInterceptor())
                .WithName("ShouldSynchronizeDeletesBetweenEntityManagers");
            CompositionContextResolver.Add(compositionContextWithSyncInterceptor);

            var rep1 = new CustomerRepository(
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(
                    "ShouldSynchronizeDeletesBetweenEntityManagers"));
            var rep2 = new CustomerRepository(
                EntityManagerProviderFactory.CreateTestEntityManagerProvider(
                    "ShouldSynchronizeDeletesBetweenEntityManagers"));

            DoItAsync(
                () =>
                {
                    ICollection<Customer> customers = new List<Customer>();
                    ICollection<Customer> customers2 = new List<Customer>();

                    var commands = new List<Func<INotifyCompleted>>
                                           {
                                               () => TestInit("ShouldSynchronizeDeletesBetweenEntityManagers"),
                                               () => rep1.GetCustomers(null, results => results.ForEach(customers.Add)),
                                               () => rep2.GetCustomers(null, results => results.ForEach(customers2.Add))
                                           };

                    CoroutineOperation coop = Coroutine.Start(commands);
                    coop.Completed +=
                        (s, args) =>
                        {
                            Customer customer = customers.First();

                            Assert.IsNotNull(
                                rep1.EntityManagerProvider.Manager.FindEntity(customer.EntityAspect.EntityKey),
                                "Customer should be in EM1's cache");
                            Assert.IsNotNull(
                                rep2.EntityManagerProvider.Manager.FindEntity(customer.EntityAspect.EntityKey),
                                "Customer should be in EM2's cache");

                            rep1.DeleteCustomer(customer);
                            Assert.IsNull(
                                rep1.EntityManagerProvider.Manager.FindEntity(customer.EntityAspect.EntityKey),
                                "Customer should have been removed from first cache");
                            Assert.IsNotNull(
                                rep2.EntityManagerProvider.Manager.FindEntity(customer.EntityAspect.EntityKey),
                                "Customer should still be in second EntityManager");

                            INotifyCompleted sop = rep1.Save();
                            sop.WhenCompleted(args1 =>
                                                  {
                                                      Assert.IsNull(
                                                          rep2.EntityManagerProvider.Manager.FindEntity(
                                                              customer.EntityAspect.EntityKey),
                                                          "Customer should have been removed from second EntityManager");
                                                      TestComplete();
                                                  });
                        };
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        [Tag("Online")]
        public void ShouldLoginLogout()
        {
            bool principalChangedFired = false;
            bool loggedInFired = false;
            bool loggedOutFired = false;
            bool managerCreatedFired = false;

            var auth = new AuthenticationService();
            CompositionContext contextWithAuthService = CompositionContext.Fake
                .WithGenerator(typeof(IAuthenticationService), () => auth)
                .WithName("ShouldLoginLogout");
            CompositionContextResolver.Add(contextWithAuthService);
            var connectionOptions =
                ConnectionOptions.Default.WithCompositionContext("ShouldLoginLogout").WithName("ShouldLoginLogout");
            ConnectionOptionsResolver.Add(connectionOptions);
            var emp = new EntityManagerProvider<NorthwindIBEntities>().With("ShouldLoginLogout");

            auth.PrincipalChanged += (s, e) => principalChangedFired = true;
            auth.LoggedIn += (s, e) => loggedInFired = true;
            auth.LoggedOut += (s, e) => loggedOutFired = true;

            emp.ManagerCreated += (s, e) => managerCreatedFired = true;
            NorthwindIBEntities manager = null;

            DoItAsync(
                () =>
                {
                    var asyncFns = new List<Func<INotifyCompleted>>
                                           {
                                               () => TestInit("ShouldLoginLogout"),
                                               () =>
                                               auth.LoginAsync(new LoginCredential("test", "test", null)),
                                               () =>
                                                   {
                                                       Assert.IsTrue(principalChangedFired,
                                                                     "PrincipalChanged should have been fired");
                                                       Assert.IsTrue(loggedInFired, "LoggedIn should have been fired");
                                                       Assert.IsFalse(loggedOutFired,
                                                                      "LoggedOut shouldn't have been fired");
                                                       Assert.IsTrue(auth.IsLoggedIn, "Should be logged in");
                                                       Assert.IsTrue(auth.Principal.Identity.Name == "test",
                                                                     "Username should be test");

                                                       manager = emp.Manager;

                                                       Assert.IsTrue(managerCreatedFired,
                                                                     "ManagerCreated should have been fired");
                                                       Assert.IsNotNull(manager.AuthenticationContext.Principal,
                                                                        "The principal should not be null on the EntitiyManager");
                                                       Assert.IsTrue(manager.AuthenticationContext.Principal.Identity.Name == "test",
                                                                     "Principal should have the same username");

                                                       principalChangedFired = false;
                                                       loggedInFired = false;
                                                       managerCreatedFired = false;
                                                       return AlwaysCompleted.Instance;
                                                   },
                                               () => auth.LogoutAsync(),
                                               () =>
                                                   {
                                                       Assert.IsTrue(principalChangedFired,
                                                                     "PrincipalChanged should have been fired");
                                                       Assert.IsFalse(loggedInFired,
                                                                      "LoggedIn shouldn't have been fired");
                                                       Assert.IsTrue(loggedOutFired, "LoggedOut should have been fired");
                                                       Assert.IsFalse(auth.IsLoggedIn, "Should be logged out");
                                                       Assert.IsNull(auth.Principal, "Principal should be null");

                                                       Assert.IsFalse(manager.IsConnected,
                                                                      "Old EntityManager should be disconnected");

                                                       manager.Customers.ExecuteAsync(
                                                           op =>
                                                               {
                                                                   Assert.IsTrue(
                                                                       op.HasError &&
                                                                       op.Error is InvalidOperationException,
                                                                       "Should have thrown an error");
                                                                   op.MarkErrorAsHandled();
                                                               });

                                                       manager = emp.Manager;
                                                       Assert.IsTrue(managerCreatedFired,
                                                                     "ManagerCreated should have been fired");
                                                       Assert.IsNull(manager.AuthenticationContext.Principal,
                                                                     "The principal should be null on the EntitiyManager");

                                                       return AlwaysCompleted.Instance;
                                                   }
                                           };

                    Coroutine.Start(asyncFns, op => TestComplete());
                });
        }

        [TestMethod]
        public void ShouldReturnFakeConnectionOptions()
        {
            var emp = new EntityManagerProvider<EntityManager>().With(ConnectionOptions.Fake.Name);
            Assert.IsTrue(emp.ConnectionOptions.IsFake);
            Assert.IsFalse(emp.ConnectionOptions.IsDesignTime);
        }

        [TestMethod]
        public void ShouldReturnDesignTimeConnectionOptions()
        {
            var emp = new EntityManagerProvider<EntityManager>().With(ConnectionOptions.DesignTime.Name);
            Assert.IsTrue(emp.ConnectionOptions.IsDesignTime);
            Assert.IsFalse(emp.ConnectionOptions.IsFake);
        }

        [TestMethod]
        public void ShouldReturnFakeStoreAuthenticationService()
        {
            var authSvc = new AuthenticationService().With(ConnectionOptions.Fake.Name);
            Assert.IsTrue(authSvc.ConnectionOptions.IsFake);
            Assert.IsFalse(authSvc.ConnectionOptions.IsDesignTime);
        }
    }
}