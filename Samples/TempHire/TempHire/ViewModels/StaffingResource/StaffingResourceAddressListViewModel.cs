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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common;
using DomainModel;
using DomainServices;
using IdeaBlade.Core;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceAddressListViewModel : StaffingResourceScreenBase
    {
        private readonly ExportFactory<ItemSelectorViewModel> _addressTypeSelectorFactory;
        private readonly IDialogManager _dialogManager;
        private readonly IUnitOfWorkManager<IResourceMgtUnitOfWork> _unitOfWorkManager;
        private BindableCollection<StaffingResourceAddressItemViewModel> _addresses;
        private BindableCollection<State> _states;

        [ImportingConstructor]
        public StaffingResourceAddressListViewModel(
            IUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
            ExportFactory<ItemSelectorViewModel> addressTypeSelectorFactory, IDialogManager dialogManager)
            : base(unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _addressTypeSelectorFactory = addressTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                    base.StaffingResource.Addresses.CollectionChanged -= AddressesCollectionChanged;

                ClearAddresses();

                if (value != null)
                {
                    Addresses =
                        new BindableCollection<StaffingResourceAddressItemViewModel>(
                            value.Addresses.ToList().Select(a => new StaffingResourceAddressItemViewModel(a, EditMode)));
                    value.Addresses.CollectionChanged += AddressesCollectionChanged;
                }

                base.StaffingResource = value;
            }
        }

        public BindableCollection<StaffingResourceAddressItemViewModel> Addresses
        {
            get { return _addresses; }
            set
            {
                _addresses = value;
                NotifyOfPropertyChange(() => Addresses);
            }
        }

        public BindableCollection<State> States
        {
            get { return _states; }
            private set
            {
                _states = value;
                NotifyOfPropertyChange(() => States);
            }
        }

        public override StaffingResourceScreenBase Start(Guid staffingResourceId, EditMode editMode)
        {
            LoadDataAsync(staffingResourceId, editMode);
            return this;
        }

        private async void LoadDataAsync(Guid staffingResourceId, EditMode editMode)
        {
            // Load the list of states once first, before we continue with starting the ViewModel
            // This is to ensure that the ComboBox binding doesn't goof up if the ItemSource is empty
            // The list of states is preloaded into each EntityManager cache, so this should be fast
            if (States == null)
            {
                var unitOfWork = _unitOfWorkManager.Get(Guid.Empty);
                var states = await unitOfWork.States.AllAsync(q => q.OrderBy(s => s.Name));
                States = new BindableCollection<State>(states);
            }

            base.Start(staffingResourceId, editMode);
        }

        private void AddressesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in
                    e.OldItems.Cast<Address>().Select(a => Addresses.First(i => i.Item == a)))
                {
                    Addresses.Remove(item);
                    item.Dispose();
                }
            }

            if (e.NewItems != null)
                e.NewItems.Cast<Address>()
                    .ForEach(a => Addresses.Add(new StaffingResourceAddressItemViewModel(a, EditMode)));

            EnsureDelete();
        }

        private void EnsureDelete()
        {
            Addresses.ForEach(i => i.NotifyOfPropertyChange(() => i.CanDelete));
        }

        public async void Add()
        {
            var addressTypes = UnitOfWork.AddressTypes;
            var addressTypeSelector = _addressTypeSelectorFactory.CreateExport().Value
                .Start("Select type:", "DisplayName", () => addressTypes.AllAsync(q => q.OrderBy(t => t.DisplayName)));

            await _dialogManager.ShowDialogAsync(addressTypeSelector, DialogButtons.OkCancel);

            StaffingResource.AddAddress((AddressType) addressTypeSelector.SelectedItem);

            EnsureDelete();
        }

        public void Delete(StaffingResourceAddressItemViewModel addressItem)
        {
            StaffingResource.DeleteAddress(addressItem.Item);

            EnsureDelete();
        }

        public void SetPrimary(StaffingResourceAddressItemViewModel addressItem)
        {
            StaffingResource.PrimaryAddress = addressItem.Item;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (!close) return;

            ClearAddresses();
        }

        private void ClearAddresses()
        {
            if (Addresses == null) return;

            // Clean up to avoid memory leaks
            Addresses.ForEach(i => i.Dispose());
            Addresses.Clear();
        }
    }
}