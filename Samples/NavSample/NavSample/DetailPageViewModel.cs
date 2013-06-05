using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace NavSample
{
    public class DetailPageViewModel : Screen, INavigationTarget
    {
        private readonly ErrorHandler _errorHandler;
        private readonly INavigator _navigator;
        private readonly ICustomerUnitOfWork _unitOfWork;
        private Customer _customer;
        private bool _restored;

        public DetailPageViewModel(INavigator navigator, ICustomerUnitOfWork unitOfWork, ErrorHandler errorHandler)
        {
            _navigator = navigator;
            _unitOfWork = unitOfWork;
            _errorHandler = errorHandler;
            _unitOfWork.EntityChanged += new EventHandler<EntityChangedEventArgs>(UnitOfWorkOnEntityChanged)
                .MakeWeak(eh => unitOfWork.EntityChanged -= eh);
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                if (Equals(value, _customer)) return;
                _customer = value;
                NotifyOfPropertyChange(() => Customer);
            }
        }

        public bool CanGoBack
        {
            get { return _navigator.CanGoBack && !_unitOfWork.HasChanges(); }
        }

        public bool CanSave
        {
            get { return _unitOfWork.HasChanges(); }
        }

        public bool CanDiscard
        {
            get { return _unitOfWork.HasChanges(); }
        }

        private void UnitOfWorkOnEntityChanged(object sender, EntityChangedEventArgs entityChangedEventArgs)
        {
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanDiscard);
            NotifyOfPropertyChange(() => CanGoBack);
        }

        public async void Start(Guid customerId)
        {
            try
            {
                if (_restored)
                    Customer = _unitOfWork.Entities.WithIdFromCache(customerId);
                else
                    Customer = await _unitOfWork.Entities.WithIdAsync(customerId);
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

        public async void Save()
        {
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }

        public void Discard()
        {
            try
            {
                _unitOfWork.Rollback();
            }
            catch (Exception e)
            {
                _errorHandler.Handle(e);
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (_unitOfWork.HasChanges())
                callback(false);
            else
                base.CanClose(callback);
        }

        public void OnNavigatedTo(NavigationArgs args)
        {
            Start((Guid)args.Parameter);
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