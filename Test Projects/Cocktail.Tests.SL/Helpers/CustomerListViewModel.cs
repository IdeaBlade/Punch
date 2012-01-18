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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using IdeaBlade.Core;
using Test.Model;

namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// An example of a ViewModel.
    /// 
    /// Notice: The ViewModel must be exported in order to be found by Caliburn Micro.
    /// </summary>
    [Export(typeof(CustomerListViewModel))]
    public class CustomerListViewModel : Screen, IDiscoverableViewModel
    {
        private readonly ICustomerRepository _repository;
        private Customer _currentCustomer;

        private BindableCollection<Customer> _customers;

        [ImportingConstructor]
        public CustomerListViewModel(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public BindableCollection<Customer> Customers
        {
            get { return _customers; }
            private set
            {
                _customers = value;
                NotifyOfPropertyChange(() => Customers);
            }
        }

        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
                NotifyOfPropertyChange(() => CurrentCustomer);
            }
        }

        private void ContextDataChanged(object sender, DataChangedEventArgs e)
        {
            MessageBox.Show(!e.EntityExists(e.EntityKeys.First()) ? "Data has been deleted" : "Data has changed");
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _repository.DataChanged += ContextDataChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _repository.DataChanged -= ContextDataChanged;
        }

        /// <summary>
        /// Start the viewModel.
        /// </summary>
        public CustomerListViewModel Start()
        {
            GetCustomers();
            return this;
        }

        public void GetCustomers()
        {
            _repository.GetCustomers("ContactName",
                                     customers =>
                                     {
                                         Customers = new BindableCollection<Customer>(customers);
                                         Customers.CollectionChanged += CustomersCollectionChanged;
                                     });
        }

        private void CustomersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems.Cast<Customer>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Delete(e.OldItems.Cast<Customer>());
                    break;
            }
        }

        private void Add(IEnumerable<Customer> customers)
        {
            customers.ForEach(_repository.AddCustomer);
        }

        private void Delete(IEnumerable<Customer> customers)
        {
            customers.ForEach(_repository.DeleteCustomer);
            Save();
        }

        public void Save()
        {
            _repository.Save();
        }
    }
}