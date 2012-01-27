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
using Common.Repositories;
using DomainModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class PhoneTypeSelectorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IStaffingResourceRepository> _repositoryManager;
        private BindableCollection<PhoneNumberType> _phoneTypes;
        private IStaffingResourceRepository _repository;
        private Guid _staffingResourceId;
        private PhoneNumberType _selectedPhoneType;

        [ImportingConstructor]
        public PhoneTypeSelectorViewModel(IRepositoryManager<IStaffingResourceRepository> repositoryManager,
                                          IErrorHandler errorHandler)
        {
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
        }

        private IStaffingResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_staffingResourceId)); }
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

        public PhoneTypeSelectorViewModel Start(Guid staffingResourceId)
        {
            _staffingResourceId = staffingResourceId;
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