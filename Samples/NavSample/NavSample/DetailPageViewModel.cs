using System;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace NavSample
{
    public class DetailPageViewModel : Screen
    {
        private readonly ErrorHandler _errorHandler;
        private readonly INavigator _navigator;
        private readonly IUnitOfWork<Customer> _unitOfWork;
        private Customer _customer;

        public DetailPageViewModel(INavigator navigator, IUnitOfWork<Customer> unitOfWork, ErrorHandler errorHandler)
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
    }
}