using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Repositories;
using DomainModel;
using IdeaBlade.Core;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceAddressListViewModel : ResourceScreenBase
    {
        private readonly ExportFactory<AddressTypeSelectorViewModel> _addressTypeSelectorFactory;
        private readonly IDialogManager _dialogManager;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private BindableCollection<ResourceAddressItemViewModel> _addresses;
        private BindableCollection<State> _states;

        [ImportingConstructor]
        public ResourceAddressListViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                            ExportFactory<AddressTypeSelectorViewModel> addressTypeSelectorFactory,
                                            IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(repositoryManager, errorHandler)
        {
            _repositoryManager = repositoryManager;
            _addressTypeSelectorFactory = addressTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public override DomainModel.Resource Resource
        {
            get { return base.Resource; }
            set
            {
                if (base.Resource != null)
                    base.Resource.Addresses.CollectionChanged -= AddressesCollectionChanged;

                ClearAddresses();

                if (value != null)
                {
                    Addresses =
                        new BindableCollection<ResourceAddressItemViewModel>(
                            value.Addresses.ToList().Select(a => new ResourceAddressItemViewModel(a)));
                    value.Addresses.CollectionChanged += AddressesCollectionChanged;
                }

                base.Resource = value;
            }
        }

        public BindableCollection<ResourceAddressItemViewModel> Addresses
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

        public override ResourceScreenBase Start(Guid resourceId)
        {
            StartCore(resourceId).ToSequentialResult().Execute();

            return this;
        }

        private IEnumerable<IResult> StartCore(Guid resourceId)
        {
            // Load the list of states once first, before we continue with starting the ViewModel
            // This is to ensure that the combobox binding doesn't goof up if the ItemSource is empty
            // The list of states is preloaded into each EntityManager cache, so this should be fast
            if (States == null)
            {
                IResourceRepository repository = _repositoryManager.GetRepository(resourceId);
                yield return repository.GetStatesAsync(result => States = new BindableCollection<State>(result),
                                                       ErrorHandler.HandleError);
            }

            base.Start(resourceId);
        }

        private void AddressesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ResourceAddressItemViewModel item in
                    e.OldItems.Cast<Address>().Select(a => Addresses.First(i => i.Item == a)))
                {
                    Addresses.Remove(item);
                    item.Dispose();
                }
            }

            if (e.NewItems != null)
                e.NewItems.Cast<Address>()
                    .ForEach(a => Addresses.Add(new ResourceAddressItemViewModel(a)));

            EnsureDelete();
        }

        private void EnsureDelete()
        {
            Addresses.ForEach(i => i.NotifyOfPropertyChange(() => i.CanDelete));
        }

        public IEnumerable<IResult> Add()
        {
            AddressTypeSelectorViewModel addressTypeSelector = _addressTypeSelectorFactory.CreateExport().Value;
            yield return _dialogManager.ShowDialog(addressTypeSelector.Start(Resource.Id));

            Resource.AddAddress(addressTypeSelector.SelectedAddressType);

            EnsureDelete();
        }

        public void Delete(ResourceAddressItemViewModel addressItem)
        {
            Resource.DeleteAddress(addressItem.Item);

            EnsureDelete();
        }

        public void SetPrimary(ResourceAddressItemViewModel addressItem)
        {
            Resource.PrimaryAddress = addressItem.Item;
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