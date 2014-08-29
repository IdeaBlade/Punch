using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.EntityModel;
using System.Linq.Expressions;

namespace NavSample
{
    public class ListPageViewModel : PageViewModel
    {
        private BindableCollection<Customer> _customers;
        private string _searchText;
        private Customer _selectedCustomer;

        // Inject Cocktail root navigation service
        public ListPageViewModel(INavigator navigator, ICustomerUnitOfWork unitOfWork, ErrorHandler errorHandler) 
            : base(navigator, unitOfWork, errorHandler)
        {
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
            get { return Navigator.CanGoBack; }
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
                    Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = q => q.OrderBy(x => x.CompanyName);

                    if (!string.IsNullOrEmpty(SearchText))
                        Search(IsRestored);
                    else
                        Customers = new BindableCollection<Customer>(
                            IsRestored ? UnitOfWork.Entities.AllInCache(orderBy) : await UnitOfWork.Entities.AllAsync(orderBy));
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        public async void GoBack()
        {
            try
            {
                await Navigator.GoBackAsync();
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        public async void Search(bool cache)
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

                    Expression<Func<Customer, bool>> predicate = x => x.CompanyName.Contains(SearchText);
                    Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = q => q.OrderBy(x => x.CompanyName);

                    Customers = new BindableCollection<Customer>(
                        cache ? UnitOfWork.Entities.FindInCache(predicate, orderBy) : await UnitOfWork.Entities.FindAsync(predicate, orderBy));
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        private async void NavigateToDetailPage()
        {
            try
            {
                // Navigate to detail page and initialize page with the selected customer.
                await Navigator.NavigateToAsync<DetailPageViewModel>(_selectedCustomer.CustomerID);
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        public override void OnNavigatedTo(NavigationArgs args)
        {
            Start();
        }

        public override void LoadState(object navigationParameter, Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            base.LoadState(navigationParameter, pageState, sharedState);

            SearchText = pageState != null ? (string)pageState["SearchText"] : null;
        }

        public override void SaveState(Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            base.SaveState(pageState, sharedState);

            pageState["SearchText"] = SearchText;
        }
    }
}