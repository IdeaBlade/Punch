using System;
using System.Linq;
using Caliburn.Micro;
using Cocktail;

namespace NavSample
{
    public class ListPageViewModel : Screen
    {
        private readonly INavigator _navigator;
        private readonly IUnitOfWork<Customer> _unitOfWork;
        private readonly ErrorHandler _errorHandler;
        private BindableCollection<Customer> _customers;
        private Customer _selectedCustomer;

        // Inject Cocktail root navigation service
        public ListPageViewModel(INavigator navigator, IUnitOfWork<Customer> unitOfWork, ErrorHandler errorHandler)
        {
            _navigator = navigator;
            _unitOfWork = unitOfWork;
            _errorHandler = errorHandler;
        }

        public BindableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                if (Equals(value, _customers)) return;
                _customers = value;
                NotifyOfPropertyChange(() => Customers);
            }
        }

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                if (Equals(value, _selectedCustomer)) return;
                _selectedCustomer = value;
                NotifyOfPropertyChange(() => SelectedCustomer);

                NavigateToDetailPage();
            }
        }

        public bool CanGoBack
        {
            get { return _navigator.CanGoBack; }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();
        }

        public async void Start()
        {
            try
            {
                var customers = await _unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.CompanyName));
                Customers = new BindableCollection<Customer>(customers);
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }

        public async void GoBack()
        {
            try
            {
                await _navigator.GoBackAsync();
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }

        private async void NavigateToDetailPage()
        {
            try
            {
                await _navigator.NavigateToAsync<DetailPageViewModel>(
                    target => target.Start(_selectedCustomer.CustomerID));
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }
    }
}