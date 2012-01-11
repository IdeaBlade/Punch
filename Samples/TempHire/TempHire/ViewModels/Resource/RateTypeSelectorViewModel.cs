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
    public class RateTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private BindableCollection<RateType> _rateTypes;
        private IResourceRepository _repository;
        private Guid _resourceId;
        private RateType _selectedRateType;

        [ImportingConstructor]
        public RateTypeSelectorViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                         IErrorHandler errorHandler)
        {
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
        }

        private IResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_resourceId)); }
        }

        public BindableCollection<RateType> RateTypes
        {
            get { return _rateTypes; }
            set
            {
                _rateTypes = value;
                NotifyOfPropertyChange(() => RateTypes);
                SelectedRateType = _rateTypes.FirstOrDefault();
            }
        }

        public RateType SelectedRateType
        {
            get { return _selectedRateType; }
            set
            {
                _selectedRateType = value;
                NotifyOfPropertyChange(() => SelectedRateType);
            }
        }

        public RateTypeSelectorViewModel Start(Guid resourceId)
        {
            _resourceId = resourceId;
            Repository.GetRateTypesAsync(result => RateTypes = new BindableCollection<RateType>(result),
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