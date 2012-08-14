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
using Cocktail.Tests.Helpers;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class UnitOfWork : CocktailTestBase
    {
        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveAllCustomers()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var expectedCount = 0;
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.CountAsync()
                                                 .ContinueWith(op => expectedCount = op.Result),
                                       () => unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.CompanyName))
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Count() == expectedCount);

                                                                       Assert.IsTrue(
                                                                           unitOfWork.Entities.CountInCache() ==
                                                                           expectedCount);

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithPredicateDescription()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);
                    var pd = PredicateBuilder.Make("City", FilterOperator.IsEqualTo, "SomeCity");

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.FindAsync(pd.ToPredicate<Customer>())
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Any());
                                                                       Assert.IsTrue(
                                                                           op.Result.All(c => c.City == "SomeCity"));

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithPredicateExpression()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.FindAsync(c => c.City == "SomeCity")
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Any());
                                                                       Assert.IsTrue(
                                                                           op.Result.All(c => c.City == "SomeCity"));

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithPredicateExpressionFromCache()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);
                    Expression<Func<Customer, bool>> expression = c => c.City == "SomeCity";

                    var entities = unitOfWork.Entities.FindInCache(expression);
                    Assert.IsTrue(!entities.Any());

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.FindInDataSourceAsync(expression)
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Any());
                                                                       Assert.IsTrue(
                                                                           op.Result.All(c => c.City == "SomeCity"));

                                                                       entities =
                                                                           unitOfWork.Entities.FindInCache(expression);
                                                                       Assert.IsTrue(entities.Count() ==
                                                                                     op.Result.Count());
                                                                       Assert.IsTrue(
                                                                           entities.All(c => c.City == "SomeCity"));

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithId()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var id = SampleDataProvider.CreateGuid(1);
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.WithIdAsync(id)
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsNotNull(op.Result);
                                                                       Assert.IsTrue(op.Result.CustomerID == id);

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithIdFromCache()
        {
            DoItAsync(
                () =>
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
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.WithIdFromDataSourceAsync(id)
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsNotNull(op.Result);
                                                                       Assert.IsTrue(unitOfWork.Entities.ExistsInCache(id)); 
                                                                       customer = unitOfWork.Entities.WithIdFromCache(id);
                                                                       Assert.IsNotNull(customer);
                                                                       Assert.IsTrue(customer.CustomerID == id);

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerIfSortedWithSortSelector()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var selector = new SortSelector("City");
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.AllAsync(q => q.OrderBySelector(selector))
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Any());

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerIfSortedWithSortFunction()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.AllAsync(q => q.OrderBy(c => c.City))
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Any());

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithSelector()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () =>
                                       unitOfWork.Entities.FindAsync(q => q.Select(x => x.CompanyName),
                                                                     x => x.City == "SomeCity",
                                                                     q => q.OrderBy(x => x))
                                           .ContinueWith(op =>
                                                             {
                                                                 Assert.IsTrue(op.CompletedSuccessfully);
                                                                 Assert.IsTrue(op.Result.Any());

                                                                 TestComplete();
                                                             })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithSelectorFromCache()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () =>
                                       unitOfWork.Entities.FindInDataSourceAsync(x => x.City == "SomeCity",
                                                                                 q => q.OrderBy(x => x.CompanyName))
                                           .ContinueWith(op =>
                                                             {
                                                                 Assert.IsTrue(op.CompletedSuccessfully);
                                                                 Assert.IsTrue(op.Result.Any());

                                                                 var names =
                                                                     unitOfWork.Entities.FindInCache(
                                                                         q => q.Select(x => x.CompanyName),
                                                                         x => x.City == "SomeCity",
                                                                         q => q.OrderBy(x => x));
                                                                 Assert.IsTrue(names.Count() == op.Result.Count());
                                                                 Assert.IsTrue(
                                                                     names.All(
                                                                         (value, index) =>
                                                                         op.Result.ElementAt(index).CompanyName == value));

                                                                 TestComplete();
                                                             })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldRetrieveCustomerWithProjectionSelector()
        {
            DoItAsync(
                () =>
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

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.FindAsync(x => x.Select(selector),
                                                                           pd.ToPredicate<Customer>(),
                                                                           q => q.OrderBySelector(sortSelector))
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.Cast<object>().Any());

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        public void ShouldCreateCustomer()
        {
            var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
            var unitOfWork = new UnitOfWork<Customer>(provider);

            var operation = unitOfWork.Factory.CreateAsync();
            Assert.IsTrue(operation.CompletedSuccessfully);
            Assert.IsNotNull(operation.Result);
            Assert.IsTrue(operation.Result.EntityAspect.EntityState.IsAdded());
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldPageCustomersWithPredicate()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var repository = new PagerRepository<Customer>(provider);

                    var sortSelector = new SortSelector("CompanyName");

                    IPager<Customer> pager = null;
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () =>
                                           {
                                               pager = repository.Pager(sortSelector, 2, x => x.City == "SomeCity");
                                               return OperationResult.FromResult(true);
                                           },
                                       () => pager.LastPageAsync()
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.PageWasFound);
                                                                       Assert.IsTrue(op.Result.Results.Count() == 1);
                                                                       Assert.IsTrue(pager.TotalItemCount == 3);
                                                                       Assert.IsTrue(pager.TotalNumberOfPages == 2);
                                                                       Assert.IsTrue(op.Result.PageIndex == 1);

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }

        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void ShouldPageProjection()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var repository = new PagerRepository<Customer>(provider);

                    var sortSelector = new SortSelector("CompanyName");

                    IPager<PageProjection> pager = null;
                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () =>
                                           {
                                               pager =
                                                   repository.Pager(
                                                       q =>
                                                       q.Select(
                                                           x =>
                                                           new PageProjection()
                                                               {CompanyName = x.CompanyName, City = x.City}), 2,
                                                       sortSelector, x => x.City == "SomeCity");
                                               return OperationResult.FromResult(true);
                                           },
                                       () => pager.FirstPageAsync()
                                                 .ContinueWith(op =>
                                                                   {
                                                                       Assert.IsTrue(op.CompletedSuccessfully);
                                                                       Assert.IsTrue(op.Result.PageWasFound);
                                                                       Assert.IsTrue(op.Result.Results.Count() == 2);
                                                                       Assert.IsTrue(op.Result.PageIndex == 0);

                                                                       TestComplete();
                                                                   })
                                   };
                    Coroutine.Start(cmds);
                });
        }
    }
}