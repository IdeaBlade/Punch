using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.EntityModel;

namespace NavSample
{
    public class ListPageViewModel : Screen, INavigationTarget
    {
        private readonly ErrorHandler _errorHandler;
        private readonly INavigator _navigator;
        private readonly ICustomerUnitOfWork _unitOfWork;
        private BindableCollection<Customer> _customers;
        private string _searchText;
        private Customer _selectedCustomer;
        private bool _restored;

        // Inject Cocktail root navigation service
        public ListPageViewModel(INavigator navigator, ICustomerUnitOfWork unitOfWork, ErrorHandler errorHandler)
        {
            _navigator = navigator;
            _unitOfWork = unitOfWork;
            _errorHandler = errorHandler;
            Busy = new BusyWatcher();
        }

        public BusyWatcher Busy { get; private set; }

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

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public async void Start()
        {
            try
            {
                using (Busy.GetTicket())
                {
                    IEnumerable<Customer> customers;
                    if (_restored)
                        customers = _unitOfWork.Entities.AllInCache(q => q.OrderBy(x => x.CompanyName));
                    else
                        customers = await _unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.CompanyName));

                    Customers = new BindableCollection<Customer>(customers);
                }
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

        public async void Search()
        {
            try
            {
                using (Busy.GetTicket())
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        Start();
                        return;
                    }

                    var customers = await _unitOfWork.Entities.FindAsync(
                        x => x.CompanyName.Contains(SearchText), q => q.OrderBy(x => x.CompanyName));
                    Customers = new BindableCollection<Customer>(customers);
                }
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
                // Navigate to detail page and initialize page with the selected customer.
                await _navigator.NavigateToAsync<DetailPageViewModel>(_selectedCustomer.CustomerID);
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }

        public void OnNavigatedTo(NavigationArgs args)
        {
            Start();
        }

        public void OnNavigatingFrom(NavigationCancelArgs args)
        {
        }

        public void OnNavigatedFrom(NavigationArgs args)
        {
        }

        public void LoadState(object navigationParameter, Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            if (!_unitOfWork.IsRestored && sharedState.ContainsKey("uow"))
                _unitOfWork.Restore((EntityCacheState)sharedState["uow"]);
        
            _restored = pageState != null;
        }

        public void SaveState(Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            sharedState["uow"] = _unitOfWork.GetCacheState();
        }

        public string PageKey { get; set; }
    }
}