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

using Test.Model;

namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// The ViewModelLocator is used during design time to find a reference to the ViewModel with
    /// sample data.
    /// 
    /// The ViewModelLocator is loaded as a static resource in App.xaml
    /// 
    /// (See) &lt:designTimeSupport:ViewModelLocator x:Key="ViewModelLocator" /&gt:
    ///  </summary>
    public class ViewModelLocator : DesignTimeViewModelLocatorBase<NorthwindIBEntities>
    {
        //TODO: Add references to all ViewModels
        public CustomerListViewModel CustomerListViewModel
        {
            get
            {
                return new CustomerListViewModel(new CustomerRepository(EntityManagerProvider)).Start();
            }
        }

        protected override IEntityManagerProvider<NorthwindIBEntities> CreateEntityManagerProvider()
        {
            return new DesignTimeEntityManagerProvider(new SampleDataProvider());
        }
    }
}