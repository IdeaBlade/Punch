using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using DomainServices;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceNameEditorViewModel : Screen
    {
        private readonly IDialogManager _dialogManager;
        private readonly IUnitOfWorkManager<IResourceMgtUnitOfWork> _unitOfWorkManager;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private IDialogUICommand<DialogResult> _okCommand;
        private Guid _staffingResourceId;
        private IResourceMgtUnitOfWork _unitOfWork;

        [ImportingConstructor]
        public StaffingResourceNameEditorViewModel(
            IUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager, IDialogManager dialogManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _dialogManager = dialogManager;
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
                UpdateCommands();
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                NotifyOfPropertyChange(() => MiddleName);
                UpdateCommands();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                UpdateCommands();
            }
        }

        private bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName); }
        }

        public StaffingResourceNameEditorViewModel Start(Guid staffingResourceId)
        {
            LoadDataAsync(staffingResourceId);
            return this;
        }

        private async void LoadDataAsync(Guid staffingResourceId)
        {
            _staffingResourceId = staffingResourceId;
            var staffingResource = await UnitOfWork.StaffingResources.WithIdAsync(_staffingResourceId);

            FirstName = staffingResource.FirstName;
            MiddleName = staffingResource.MiddleName;
            LastName = staffingResource.LastName;
        }

        public Task<DialogResult> ShowDialogAsync()
        {
            _okCommand = new DialogUICommand<DialogResult>(DialogResult.Ok, true) {Enabled = IsComplete};
            var cancelCommand = new DialogUICommand<DialogResult>(DialogResult.Cancel, false, true);

            return _dialogManager.ShowDialogAsync(new[] {_okCommand, cancelCommand}, this);
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

        private void UpdateCommands()
        {
            if (_okCommand != null)
                _okCommand.Enabled = IsComplete;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                _unitOfWork = null;
        }
    }
}