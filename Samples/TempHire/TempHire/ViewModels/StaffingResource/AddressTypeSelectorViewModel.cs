//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Errors;
using Common.Factories;
using Common.Repositories;
using DomainModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddressTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IStaffingResourceRepository> _repositoryManager;
        private BindableCollection<AddressType> _addressTypes;
        private IStaffingResourceRepository _repository;
        private AddressType _selectedAddressType;
        private Guid _staffingResourceId;

        [ImportingConstructor]
        public AddressTypeSelectorViewModel(IRepositoryManager<IStaffingResourceRepository> repositoryManager,
                                            IErrorHandler errorHandler)
        {
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
        }

        private IStaffingResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_staffingResourceId)); }
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

        public AddressTypeSelectorViewModel Start(Guid staffingResourceId)
        {
            _staffingResourceId = staffingResourceId;
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

    [Export(typeof(IPartFactory<AddressTypeSelectorViewModel>))]
    public class AddressTypeSelectorViewModelFactory : PartFactoryBase<AddressTypeSelectorViewModel>
    {
    }
}