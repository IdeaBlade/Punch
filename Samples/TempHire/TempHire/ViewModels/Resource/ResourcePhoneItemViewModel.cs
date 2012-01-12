using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;
using DomainModel;

namespace TempHire.ViewModels.Resource
{
    public class ResourcePhoneItemViewModel : PropertyChangedBase, IDisposable
    {
        private PhoneNumber _item;

        public ResourcePhoneItemViewModel(PhoneNumber item)
        {
            Debug.Assert(item != null);
            Item = item;
        }

        public PhoneNumber Item
        {
            get { return _item; }
            private set
            {
                _item = value;
                _item.EntityFacts.EntityPropertyChanged += ItemPropertyChanged;

                NotifyOfPropertyChange(() => Item);
            }
        }

        public bool CanDelete
        {
            get { return !Item.Primary && (Item.Resource.PhoneNumbers.Count > 1); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _item.EntityFacts.EntityPropertyChanged -= ItemPropertyChanged;
            _item = null;
        }

        #endregion

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == PhoneNumber.EntityPropertyNames.Primary)
                NotifyOfPropertyChange(() => CanDelete);
        }
    }
}