using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace NavSample
{
    public class DetailPageViewModel : PageViewModel
    {
        private Customer _customer;

        public DetailPageViewModel(INavigator navigator, ICustomerUnitOfWork unitOfWork, ErrorHandler errorHandler)
            : base(navigator, unitOfWork, errorHandler)
        {
            UnitOfWork.EntityChanged += new EventHandler<EntityChangedEventArgs>(UnitOfWorkOnEntityChanged)
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
            get { return Navigator.CanGoBack && !UnitOfWork.HasChanges(); }
        }

        public bool CanSave
        {
            get { return UnitOfWork.HasChanges(); }
        }

        public bool CanDiscard
        {
            get { return UnitOfWork.HasChanges(); }
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
                if (IsRestored)
                    Customer = UnitOfWork.Entities.WithIdFromCache(customerId);
                else
                    Customer = await UnitOfWork.Entities.WithIdAsync(customerId);
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

        public async void Save()
        {
            try
            {
                await UnitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        public void Discard()
        {
            try
            {
                UnitOfWork.Rollback();
            }
            catch (Exception e)
            {
                ErrorHandler.Handle(e);
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (UnitOfWork.HasChanges())
                callback(false);
            else
                base.CanClose(callback);
        }

        public override void OnNavigatedTo(NavigationArgs args)
        {
            Start((Guid)args.Parameter);
        }
    }
}