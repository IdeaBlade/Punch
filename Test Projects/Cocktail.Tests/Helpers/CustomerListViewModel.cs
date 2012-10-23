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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;
using Test.Model;

#if !NETFX_CORE
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail.Tests.Helpers
{
    public class CustomerListViewModel : Screen, IDiscoverableViewModel
    {
        private readonly ICustomerRepository _repository;
        private Customer _currentCustomer;

        private ObservableCollection<Customer> _customers;

        [ImportingConstructor]
        public CustomerListViewModel(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public ObservableCollection<Customer> Customers
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

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();
        }

        /// <summary>
        /// Start the viewModel.
        /// </summary>
        public CustomerListViewModel Start()
        {
            GetCustomers();
            return this;
        }

        public async void GetCustomers()
        {
            var customers = await _repository.GetCustomersAsync("ContactName");

            Customers = new ObservableCollection<Customer>(customers);
            Customers.CollectionChanged += CustomersCollectionChanged;
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
            _repository.SaveAsync();
        }
    }
}