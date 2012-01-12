using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;
using DomainModel;

namespace TempHire.ViewModels.Resource
{
    public class ResourceAddressItemViewModel : PropertyChangedBase, IDisposable
    {
        private Address _item;

        public ResourceAddressItemViewModel(Address item)
        {
            Debug.Assert(item != null);
            Item = item;
        }

        public Address Item
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
            get { return !Item.Primary && (Item.Resource.Addresses.Count > 1); }
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
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == Address.EntityPropertyNames.Primary)
                NotifyOfPropertyChange(() => CanDelete);
        }
    }
}