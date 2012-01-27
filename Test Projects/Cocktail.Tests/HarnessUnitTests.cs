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

using System.ComponentModel.Composition;
using System.Linq;
using Cocktail.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Model;

namespace Cocktail.Tests
{
    [TestClass]
    public class HarnessUnitTests
    {
        //private readonly ViewModelLocator _locator;

        public HarnessUnitTests()
        {
            Composition.Configure();
            //_locator = new ViewModelLocator();
        }

        //[Export]
        //public IEntityManagerProvider<NorthwindIBEntities> EntityManagerProvider
        //{
        //    get { return new DesignTimeEntityManagerProvider(new SampleDataProvider()); }
        //}

        //[TestMethod]
        //public void ShouldGetCustomersInDesignMode()
        //{
        //    DesignTimeViewModelLocatorBase<NorthwindIBEntities>.IsInDesignMode = () => true;
        //    Composition.IsInDesignMode = () => true;

        //    Assert.IsTrue(_locator.CustomerListViewModel != null, "The ViewModel should be set");

        //    CustomerListViewModel viewModel = _locator.CustomerListViewModel;
        //    //viewModel.GetCustomers();

        //    Assert.IsTrue(viewModel != null && viewModel.Customers.Count > 0, "We should have at least 1 customer");
        //}

        //[TestMethod]
        //public void ShouldGetViewModelList()
        //{
        //    Composition.ResetIsInDesignModeToDefault();

        //    var shell = new HarnessViewModel(Composition.GetInstances<IDiscoverableViewModel>());
        //    Composition.BuildUp(shell);

        //    Assert.IsTrue(shell.Names.Any(), "We should have at least one ViewModel name");
        //}
    }
}