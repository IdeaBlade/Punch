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
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using DomainServices;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceNameEditorViewModel : Screen
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> _unitOfWorkManager;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private DialogButton _okButton;
        private IResourceMgtUnitOfWork _unitOfWork;
        private Guid _staffingResourceId;

        [ImportingConstructor]
        public StaffingResourceNameEditorViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                                   IErrorHandler errorHandler)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _errorHandler = errorHandler;
        }

        private IResourceMgtUnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (_unitOfWork = _unitOfWorkManager.Get(_staffingResourceId)); }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                OnCompleteChanged();
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                NotifyOfPropertyChange(() => MiddleName);
                OnCompleteChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                OnCompleteChanged();
            }
        }

        public bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName); }
        }

        public StaffingResourceNameEditorViewModel Start(Guid staffingResourceId)
        {
            _staffingResourceId = staffingResourceId;
            UnitOfWork.StaffingResources.WithIdAsync(_staffingResourceId,
                                                     result =>
                                                         {
                                                             FirstName = result.FirstName;
                                                             MiddleName = result.MiddleName;
                                                             LastName = result.LastName;
                                                         },
                                                     _errorHandler.HandleError);
            return this;
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!this.DialogHost().DialogResult.Equals(DialogResult.Cancel))
            {
                callback(IsComplete);
            }
            else
                base.CanClose(callback);
        }

        private void OnCompleteChanged()
        {
            if (_okButton != null)
                _okButton.Enabled = IsComplete;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                _unitOfWork = null;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _okButton = this.DialogHost().GetButton(DialogResult.Ok);
            _okButton.Enabled = IsComplete;
        }
    }
}