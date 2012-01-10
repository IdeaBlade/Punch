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

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro.Extensions;
using IdeaBlade.Application.Framework.Core.Composition;
using IdeaBlade.Application.Framework.Core.DesignTimeSupport;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.Application.Framework.Core.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApplication.DesignTimeSupport;
using SampleApplication.ViewModel;
using SampleModel;

namespace Core.Tests.SL
{
    [TestClass]
    public class HarnessUnitTests
    {
        private readonly ViewModelLocator _locator;

        public HarnessUnitTests()
        {
            CompositionHelper.Configure();
            _locator = new ViewModelLocator();
        }

        [Export]
        public IEntityManagerProvider<NorthwindIBEntities> EntityManagerProvider
        {
            get { return new DesignTimeEntityManagerProvider(new SampleDataProvider()); }
        }

        [TestMethod]
        public void ShouldGetCustomersInDesignMode()
        {
            BaseDesignTimeViewModelLocator<NorthwindIBEntities>.IsInDesignMode = delegate { return true; };
            CompositionHelper.IsInDesignMode = delegate { return true; };

            Assert.IsTrue(_locator.CustomerListViewModel != null, "The ViewModel should be set");

            CustomerListViewModel viewModel = _locator.CustomerListViewModel;
            //viewModel.GetCustomers();

            Assert.IsTrue(viewModel.Customers.Count > 0, "We should have at least 1 customer");
        }

        [TestMethod]
        public void ShouldGetViewModelList()
        {
            CompositionHelper.ResetIsInDesignModeToDefault();

            var shell = new HarnessViewModel(CompositionHelper.GetInstances<IDiscoverableViewModel>(), null);
            CompositionHelper.BuildUp(shell);

            Assert.IsTrue(shell.Names.Any(), "We should have at least one ViewModel name");
        }
    }
}