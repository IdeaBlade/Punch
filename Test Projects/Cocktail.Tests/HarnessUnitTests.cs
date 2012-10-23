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

using System.Linq;
using Cocktail.Tests.Helpers;
using Test.Model;

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
    public class HarnessUnitTests : CocktailTestBase
    {
        private ViewModelLocator _locator;

        protected override void Context()
        {
            base.Context();
            _locator = new ViewModelLocator();
        }

        [TestMethod]
        public void ShouldGetCustomersInDesignMode()
        {
            DesignTime.InDesignMode = () => true;

            var vm = _locator.CustomerListViewModel;
            Assert.IsNotNull(vm, "The ViewModel should be set");

            CustomerListViewModel viewModel = _locator.CustomerListViewModel;
            //viewModel.GetCustomers();

            Assert.IsTrue(viewModel != null && viewModel.Customers.Count > 0, "We should have at least 1 customer");
        }

        [TestMethod]
        public void ShouldGetViewModelList()
        {
#if NETFX_CORE
            var shell = new HarnessViewModel(null, Composition.GetInstances<IDiscoverableViewModel>());
#else
            var shell = new HarnessViewModel(Composition.GetInstances<IDiscoverableViewModel>());
#endif
            Composition.BuildUp(shell);

            Assert.IsTrue(shell.Names.Any(), "We should have at least one ViewModel name");
        }
    }
}