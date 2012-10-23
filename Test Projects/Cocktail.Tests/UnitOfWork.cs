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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Test.Model;
using CompositionContext = IdeaBlade.Core.Composition.CompositionContext;

#if !NETFX_CORE
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using System.Composition;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Cocktail.Tests
{
    [TestClass]
    public class UnitOfWork : CocktailTestBase
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveAllCustomers()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var expectedCount = await unitOfWork.Entities.CountAsync();
            var customers = await unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.CompanyName));

            Assert.IsTrue(customers.Count() == expectedCount);
            Assert.IsTrue(unitOfWork.Entities.CountInCache() == expectedCount);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveAllCustomersAndOrders()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            unitOfWork.Clear();
            var customers = await unitOfWork.Entities.AllAsync();
            Assert.IsTrue(customers.Any());
            Assert.IsFalse(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllAsync(includeProperties: "Orders");
            Assert.IsTrue(customers.Any());
            Assert.IsTrue(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllAsync(CancellationToken.None);
            Assert.IsTrue(customers.Any());
            Assert.IsFalse(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllAsync(CancellationToken.None, includeProperties: "Orders");
            Assert.IsTrue(customers.Any());
            Assert.IsTrue(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllInDataSourceAsync();
            Assert.IsTrue(customers.Any());
            Assert.IsFalse(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllInDataSourceAsync(includeProperties: "Orders");
            Assert.IsTrue(customers.Any());
            Assert.IsTrue(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllInDataSourceAsync(CancellationToken.None);
            Assert.IsTrue(customers.Any());
            Assert.IsFalse(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());

            unitOfWork.Clear();
            customers = await unitOfWork.Entities.AllInDataSourceAsync(CancellationToken.None, includeProperties: "Orders");
            Assert.IsTrue(customers.Any());
            Assert.IsTrue(provider.Manager.FindEntities<Order>(EntityState.AllButDetached).Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithPredicateDescription()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);
            var pd = PredicateBuilder.Make("City", FilterOperator.IsEqualTo, "SomeCity");

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindAsync(pd.ToPredicate<Customer>());

            Assert.IsTrue(customers.Any());
            Assert.IsTrue(customers.All(c => c.City == "SomeCity"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithPredicateExpression()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindAsync(c => c.City == "SomeCity");

            Assert.IsTrue(customers.Any());
            Assert.IsTrue(customers.All(c => c.City == "SomeCity"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithPredicateExpressionFromCache()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);
            Expression<Func<Customer, bool>> expression = c => c.City == "SomeCity";

            var entities = unitOfWork.Entities.FindInCache(expression);
            Assert.IsFalse(entities.Any());

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var result = await unitOfWork.Entities.FindInDataSourceAsync(expression);

            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.All(c => c.City == "SomeCity"));

            entities = unitOfWork.Entities.FindInCache(expression);
            Assert.IsTrue(entities.Count() == result.Count());
            Assert.IsTrue(entities.All(c => c.City == "SomeCity"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomerWithId()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var id = SampleDataProvider.CreateGuid(1);
            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customer = await unitOfWork.Entities.WithIdAsync(id);

            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.CustomerID == id);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomerWithIdFromCache()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var id = SampleDataProvider.CreateGuid(1);
            Assert.IsFalse(unitOfWork.Entities.ExistsInCache(id));
            Customer customer = null;
            try
            {
                customer = unitOfWork.Entities.WithIdFromCache(id);
            }
            catch (EntityNotFoundException)
            {
                // Expected exception
            }
            Assert.IsNull(customer);

            // Fetch from data source
            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            await unitOfWork.Entities.WithIdFromDataSourceAsync(id);

            Assert.IsTrue(unitOfWork.Entities.ExistsInCache(id));
            customer = unitOfWork.Entities.WithIdFromCache(id);
            Assert.IsNotNull(customer);
            Assert.IsTrue(customer.CustomerID == id);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersIfSortedWithSortSelector()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var selector = new SortSelector("City");
            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.AllAsync(q => q.OrderBySelector(selector));

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersIfSortedWithSortFunction()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.AllAsync(q => q.OrderBy(c => c.City));

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithSelector()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var result = await unitOfWork.Entities.FindAsync(q => q.Select(x => x.CompanyName),
                                                                     x => x.City == "SomeCity",
                                                                     q => q.OrderBy(x => x));

            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithSelectorFromCache()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindInDataSourceAsync(x => x.City == "SomeCity",
                                                                                 q => q.OrderBy(x => x.CompanyName));

            Assert.IsTrue(customers.Any());

            var names = unitOfWork.Entities.FindInCache(
                            q => q.Select(x => x.CompanyName),
                            x => x.City == "SomeCity",
                            q => q.OrderBy(x => x));
            Assert.IsTrue(names.Count() == customers.Count());
            Assert.IsTrue(
                names.All((value, index) => customers.ElementAt(index).CompanyName == value));

        }

#if !NETFX_CORE
        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldRetrieveCustomersWithProjectionSelector()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var selector =
                ProjectionSelector.Combine(new[]
                                        {
                                            new ProjectionSelector("CustomerID"),
                                            new ProjectionSelector("CompanyName")
                                        });
            var pd = PredicateBuilder.Make("City", FilterOperator.IsEqualTo, "SomeCity");
            var sortSelector = new SortSelector("CompanyName");

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var result = await unitOfWork.Entities.FindAsync(q => q.Select(selector),
                                                             pd.ToPredicate<Customer>(),
                                                             q => q.OrderBySelector(sortSelector));

            Assert.IsTrue(result.Cast<object>().Any());
        }
#endif

        [TestMethod]
        public void ShouldCreateCustomer()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var task = unitOfWork.Factory.CreateAsync();
            Assert.IsNotNull(task.Result);
            Assert.IsTrue(task.Result.EntityAspect.EntityState.IsAdded());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldPageCustomersWithPredicate()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var repository = new PagerRepository<Customer>(provider);

            var sortSelector = new SortSelector("CompanyName");

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var pager = repository.Pager(sortSelector, 2, x => x.City == "SomeCity");
            var page = await pager.LastPageAsync();

            Assert.IsTrue(page.PageWasFound);
            Assert.IsTrue(page.Results.Count() == 1);
            Assert.IsTrue(pager.TotalItemCount == 3);
            Assert.IsTrue(pager.TotalNumberOfPages == 2);
            Assert.IsTrue(page.PageIndex == 1);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldPageProjection()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var repository = new PagerRepository<Customer>(provider);

            var sortSelector = new SortSelector("CompanyName");

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var pager = repository.Pager(
                q => q.Select(x => new PageProjection() { CompanyName = x.CompanyName, City = x.City }),
                2, sortSelector, x => x.City == "SomeCity");
            var page = await pager.FirstPageAsync();

            Assert.IsTrue(page.PageWasFound);
            Assert.IsTrue(page.Results.Count() == 2);
            Assert.IsTrue(page.PageIndex == 0);
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldCancelQueryTask()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);
            var em = provider.Manager;
            em.Querying += (sender, args) => args.Cancel = true;

            await unitOfWork.Entities.AllAsync()
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldCancelQueryTaskWithToken()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var cts = new CancellationTokenSource();
            cts.Cancel();
            await unitOfWork.Entities.AllAsync(cts.Token)
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));

            await unitOfWork.Entities.AllInDataSourceAsync(cts.Token)
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldCancelProjectionQueryTask()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var cts = new CancellationTokenSource();
            cts.Cancel();
            await unitOfWork.Entities.FindAsync(q => q.Select(x => x.CompanyName), cts.Token)
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));

            await unitOfWork.Entities.FindInDataSourceAsync(q => q.Select(x => x.CompanyName), cts.Token)
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task ShouldCancelSaveTask()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);
            var em = provider.Manager;

            var customer = new Customer { CustomerID = Guid.NewGuid(), CompanyName = "Foo" };
            em.AddEntity(customer);
            em.Saving += (sender, args) =>
                args.Cancel = true;

            Assert.IsTrue(em.HasChanges());
            await unitOfWork.CommitAsync()
                .ContinueWith(task => Assert.IsTrue(task.IsCanceled, "Should be cancelled"));
        }

        [TestMethod]
        public void ShouldCreateEntityWithInternalCtor()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<EntityWithInternalCtor>(provider);

            var task = unitOfWork.Factory.CreateAsync();
            Assert.IsNotNull(task.Result);
            Assert.IsTrue(task.Result.EntityAspect.EntityState.IsAdded());
        }

        [TestMethod]
        public void ShouldCreateEntityWithInternalFactoryMethod()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<EntityWithInternalFactoryMethod>(provider);

            var task = unitOfWork.Factory.CreateAsync();
            Assert.IsNotNull(task.Result);
            Assert.IsTrue(task.Result.Id == 100);
            Assert.IsTrue(task.Result.EntityAspect.EntityState.IsAdded());
        }

        [TestMethod]
        public void ShouldCreateEntityWithPublicFactoryMethod()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<EntityWithPublicFactoryMethod>(provider);

            var task = unitOfWork.Factory.CreateAsync();
            Assert.IsNotNull(task.Result);
            Assert.IsTrue(task.Result.Id == 200);
            Assert.IsTrue(task.Result.EntityAspect.EntityState.IsAdded());
        }
    }
}