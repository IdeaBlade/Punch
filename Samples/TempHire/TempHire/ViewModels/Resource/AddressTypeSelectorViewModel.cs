using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using Common.Repositories;
using DomainModel;
using TempHire.Repositories;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddressTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private BindableCollection<AddressType> _addressTypes;
        private IResourceRepository _repository;
        private Guid _resourceId;
        private AddressType _selectedAddressType;

        [ImportingConstructor]
        public AddressTypeSelectorViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                            IErrorHandler errorHandler)
        {
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
        }

        private IResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_resourceId)); }
        }

        public BindableCollection<AddressType> AddressTypes
        {
            get { return _addressTypes; }
            set
            {
                _addressTypes = value;
                NotifyOfPropertyChange(() => AddressTypes);
                SelectedAddressType = _addressTypes.FirstOrDefault();
            }
        }

        public AddressType SelectedAddressType
        {
            get { return _selectedAddressType; }
            set
            {
                _selectedAddressType = value;
                NotifyOfPropertyChange(() => SelectedAddressType);
            }
        }

        public AddressTypeSelectorViewModel Start(Guid resourceId)
        {
            _resourceId = resourceId;
            Repository.GetAddressTypesAsync(result => AddressTypes = new BindableCollection<AddressType>(result),
                                            _errorHandler.HandleError);
            return this;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                _repository = null;
        }
    }
}