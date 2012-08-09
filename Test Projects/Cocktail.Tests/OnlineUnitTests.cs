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
using System.Threading.Tasks;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class OnlineUnitTests : CocktailTestBase
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldGetCustomers()
        {
            var emp = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var repository = new CustomerRepository(emp);

            await InitFakeBackingStoreAsync(emp.Manager.CompositionContext.Name);
            var customers = await repository.GetCustomersAsync(null);

            Assert.IsTrue(customers.Any(), "Should have some customers");
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldSynchronizeDeletesBetweenEntityManagers()
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

            await InitFakeBackingStoreAsync("ShouldSynchronizeDeletesBetweenEntityManagers");
            var customers = await rep1.GetCustomersAsync(null);
            var customers2 = await rep2.GetCustomersAsync(null);

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

            await rep1.SaveAsync();
            Assert.IsNull(rep2.EntityManagerProvider.Manager.FindEntity(customer.EntityAspect.EntityKey),
                "Customer should have been removed from second EntityManager");
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldLoginLogout()
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
            var emp = new EntityManagerProvider<NorthwindIBEntities>()
                .Configure(provider => provider.WithConnectionOptions("ShouldLoginLogout"));

            auth.PrincipalChanged += (s, e) => principalChangedFired = true;
            auth.LoggedIn += (s, e) => loggedInFired = true;
            auth.LoggedOut += (s, e) => loggedOutFired = true;

            emp.ManagerCreated += (s, e) => managerCreatedFired = true;
            NorthwindIBEntities manager = null;

            await InitFakeBackingStoreAsync("ShouldLoginLogout");
            await auth.LoginAsync(new LoginCredential("test", "test", null));
                                                       
            Assert.IsTrue(principalChangedFired, "PrincipalChanged should have been fired");
            Assert.IsTrue(loggedInFired, "LoggedIn should have been fired");
            Assert.IsFalse(loggedOutFired, "LoggedOut shouldn't have been fired");
            Assert.IsTrue(auth.IsLoggedIn, "Should be logged in");
            Assert.IsTrue(auth.Principal.Identity.Name == "test", "Username should be test");

            manager = emp.Manager;

            Assert.IsTrue(managerCreatedFired, "ManagerCreated should have been fired");
            Assert.IsNotNull(manager.AuthenticationContext.Principal,
                            "The principal should not be null on the EntitiyManager");
            Assert.IsTrue(manager.AuthenticationContext.Principal.Identity.Name == "test",
                            "Principal should have the same username");

            principalChangedFired = false;
            loggedInFired = false;
            managerCreatedFired = false;

            await auth.LogoutAsync();

            Assert.IsTrue(principalChangedFired,
                          "PrincipalChanged should have been fired");
            Assert.IsFalse(loggedInFired,
                           "LoggedIn shouldn't have been fired");
            Assert.IsTrue(loggedOutFired, "LoggedOut should have been fired");
            Assert.IsFalse(auth.IsLoggedIn, "Should be logged out");
            Assert.IsNull(auth.Principal, "Principal should be null");

            Assert.IsFalse(manager.IsConnected,
                           "Old EntityManager should be disconnected");

            bool exceptionThrown = false;
            try
            {
                await manager.Customers.ExecuteAsync();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Should have thrown an error");

            manager = emp.Manager;
            Assert.IsTrue(managerCreatedFired,
                          "ManagerCreated should have been fired");
            Assert.IsNull(manager.AuthenticationContext.Principal,
                          "The principal should be null on the EntitiyManager");

        }

        [TestMethod]
        public void ShouldReturnFakeConnectionOptions()
        {
            var emp = new EntityManagerProvider<EntityManager>()
                .Configure(provider => provider.WithConnectionOptions(ConnectionOptions.Fake.Name));
            Assert.IsTrue(emp.ConnectionOptions.IsFake);
            Assert.IsFalse(emp.ConnectionOptions.IsDesignTime);
        }

        [TestMethod]
        public void ShouldReturnDesignTimeConnectionOptions()
        {
            var emp = new EntityManagerProvider<EntityManager>()
                .Configure(provider => provider.WithConnectionOptions(ConnectionOptions.DesignTime.Name));
            Assert.IsTrue(emp.ConnectionOptions.IsDesignTime);
            Assert.IsFalse(emp.ConnectionOptions.IsFake);
        }

        [TestMethod]
        public void ShouldReturnFakeStoreAuthenticationService()
        {
            var authSvc = new AuthenticationService()
                .Configure(auth => auth.WithConnectionOptions(ConnectionOptions.Fake.Name));
            Assert.IsTrue(authSvc.ConnectionOptions.IsFake);
            Assert.IsFalse(authSvc.ConnectionOptions.IsDesignTime);
        }
    }
}