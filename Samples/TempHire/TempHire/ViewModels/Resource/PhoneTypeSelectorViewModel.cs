using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using Common.Repositories;
using DomainModel;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class PhoneTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private BindableCollection<PhoneNumberType> _phoneTypes;
        private IResourceRepository _repository;
        private Guid _resourceId;
        private PhoneNumberType _selectedPhoneType;

        [ImportingConstructor]
        public PhoneTypeSelectorViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                          IErrorHandler errorHandler)
        {
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
        }

        private IResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_resourceId)); }
        }

        public BindableCollection<PhoneNumberType> PhoneTypes
        {
            get { return _phoneTypes; }
            set
            {
                _phoneTypes = value;
                NotifyOfPropertyChange(() => PhoneTypes);
                SelectedPhoneType = _phoneTypes.FirstOrDefault();
            }
        }

        public PhoneNumberType SelectedPhoneType
        {
            get { return _selectedPhoneType; }
            set
            {
                _selectedPhoneType = value;
                NotifyOfPropertyChange(() => SelectedPhoneType);
            }
        }

        public PhoneTypeSelectorViewModel Start(Guid resourceId)
        {
            _resourceId = resourceId;
            Repository.GetPhoneTypesAsync(results => PhoneTypes = new BindableCollection<PhoneNumberType>(results),
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