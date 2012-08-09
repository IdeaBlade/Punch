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
using System.Threading.Tasks;
using Cocktail.Tests.Helpers;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class ShouldRetrieveCustomers : CocktailTestBase
    {
        [TestMethod]
        [Timeout(10000)]
        public async Task WithPredicateDescription()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);
            var pd = PredicateBuilder.Make("City", FilterOperator.IsEqualTo, "SomeCity");

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindAsync(pd);

            Assert.IsTrue(customers.Any());
            Assert.IsTrue(customers.All(c => c.City == "SomeCity"));
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task WithPredicateExpression()
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
        public async Task WithPredicateExpressionFromCache()
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
        public async Task WithId()
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
        public async Task WithIdFromCache()
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
        public async Task IfSortedWithSortSelector()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var selector = new SortSelector("City");
            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindAsync(sortSelector: selector);

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task IfSortedWithSortFunction()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            await InitFakeBackingStoreAsync(CompositionContext.Fake.Name);
            var customers = await unitOfWork.Entities.FindAsync(orderBy: q => q.OrderBy(c => c.City));

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        [Timeout(10000)]
        public async Task WithSelector()
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
        public async Task WithSelectorFromCache()
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

        [TestMethod]
        [Timeout(10000)]
        public async Task WithProjectionSelector()
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
            var result = await unitOfWork.Entities.FindAsync(selector, pd, sortSelector);

            Assert.IsTrue(result.Cast<object>().Any());
        }
    }
}