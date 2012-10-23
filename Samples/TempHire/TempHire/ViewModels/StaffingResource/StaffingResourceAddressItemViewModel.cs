// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;
using Common;
using DomainModel;

namespace TempHire.ViewModels.StaffingResource
{
    public class StaffingResourceAddressItemViewModel : PropertyChangedBase, IDisposable
    {
        private readonly EditMode _editMode;
        private Address _item;

        public StaffingResourceAddressItemViewModel(Address item, EditMode editMode)
        {
            _editMode = editMode;
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

        public bool IsReadOnly
        {
            get { return _editMode == EditMode.View; }
        }

        public bool CanDelete
        {
            get { return !IsReadOnly && !Item.Primary && (Item.StaffingResource.Addresses.Count > 1); }
        }

        public bool CanSetPrimary
        {
            get { return !IsReadOnly && !Item.Primary; }
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
            {
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => CanSetPrimary);
            }
        }
    }
}