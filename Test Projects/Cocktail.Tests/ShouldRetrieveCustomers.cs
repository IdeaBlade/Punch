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
    public class ShouldRetrieveCustomers : CocktailTestBase
    {
        [TestMethod]
        [Asynchronous, Timeout(10000)]
        public void WithPredicateDescription()
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
                                       () => unitOfWork.Entities.FindAsync(pd)
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
        public void WithPredicateExpression()
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
        public void WithPredicateExpressionFromCache()
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
        public void WithId()
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
        public void WithIdFromCache()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var id = SampleDataProvider.CreateGuid(1);
                    Customer customer = null;
                    try
                    {
                        customer = unitOfWork.Entities.WithIdFromCache(id);
                    }
                    catch (EntityServerException)
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
        public void IfSortedWithSortSelector()
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
                                       () => unitOfWork.Entities.FindAsync(sortSelector: selector)
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
        public void IfSortedWithSortFunction()
        {
            DoItAsync(
                () =>
                {
                    var provider = EntityManagerProviderFactory.CreateTestEntityManagerProvider();
                    var unitOfWork = new UnitOfWork<Customer>(provider);

                    var cmds = new List<Func<INotifyCompleted>>
                                   {
                                       () => TestInit(CompositionContext.Fake.Name),
                                       () => unitOfWork.Entities.FindAsync(orderBy: q => q.OrderBy(c => c.City))
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
        public void WithSelector()
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
        public void WithSelectorFromCache()
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
        public void WithProjectionSelector()
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
                                       () => unitOfWork.Entities.FindAsync(selector, pd, sortSelector)
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
    }
}