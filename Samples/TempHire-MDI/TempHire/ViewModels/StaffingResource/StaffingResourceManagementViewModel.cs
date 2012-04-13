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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Factories;
using Common.Forms;
using Common.Messages;
using Common.Toolbar;
using Common.Workspace;
using DomainModel.Projections;
using DomainServices;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace TempHire.ViewModels.StaffingResource
{
    [Export]
    public class StaffingResourceManagementViewModel : Conductor<IScreen>.Collection.OneActive, IDiscoverableViewModel,
                                                       IHandle<EntityChangedMessage>,
                                                       IWorkspace
    {
        private readonly IDialogManager _dialogManager;
        private readonly IErrorHandler _errorHandler;
        private readonly IPartFactory<StaffingResourceNameEditorViewModel> _nameEditorFactory;
        private readonly IToolbarManager _toolbar;
        private readonly IDomainUnitOfWorkManager<IDomainUnitOfWork> _unitOfWorkManager;
        private ToolbarGroup _toolbarGroup;
        private readonly IFormsManager<StaffingResourceDetailViewModel> _forms;

        [ImportingConstructor]
        public StaffingResourceManagementViewModel(StaffingResourceSearchViewModel searchPane,
                                                   IPartFactory<StaffingResourceNameEditorViewModel> nameEditorFactory,
                                                   IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                                   IErrorHandler errorHandler, IDialogManager dialogManager,
                                                   IToolbarManager toolbar)
        {
            SearchPane = searchPane;
            _nameEditorFactory = nameEditorFactory;
            _unitOfWorkManager = unitOfWorkManager;
            _errorHandler = errorHandler;
            _dialogManager = dialogManager;
            _toolbar = toolbar;

            PropertyChanged += OnPropertyChanged;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Resource Management";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            _forms = new FormsManager<StaffingResourceDetailViewModel>();
        }

        public StaffingResourceSearchViewModel SearchPane { get; private set; }

        public bool CanDelete
        {
            get { return SearchPane.CurrentStaffingResource != null; }
        }

        private IDomainUnitOfWork ActiveUnitOfWork
        {
            get { return _unitOfWorkManager.Get(ActiveStaffingResource.Id); }
        }

        private StaffingResourceDetailViewModel ActiveDetail
        {
            get { return ActiveItem as StaffingResourceDetailViewModel; }
        }

        private DomainModel.StaffingResource ActiveStaffingResource
        {
            get { return ActiveDetail != null ? ActiveDetail.StaffingResource : null; }
        }

        public bool CanSave
        {
            get
            {
                return ActiveStaffingResource != null && ActiveUnitOfWork.HasChanges() &&
                       !ActiveStaffingResource.EntityFacts.EntityState.IsDeleted();
            }
        }

        public bool CanCancel
        {
            get { return CanSave; }
        }

        #region IHandle<EntityChangedMessage> Members

        public void Handle(EntityChangedMessage message)
        {
            if (ActiveStaffingResource == null || !ActiveUnitOfWork.HasEntity(message.Entity)) 
                return;

            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanCancel);
        }

        #endregion

        #region IWorkspace Members

        public bool IsDefault
        {
            get { return false; }
        }

        public int Sequence
        {
            get { return 10; }
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveItem")
            {
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanCancel);

                if (ActiveStaffingResource == null) return;

                SearchPane.CurrentStaffingResource =
                    SearchPane.Items.FirstOrDefault(resource => resource.Id == ActiveStaffingResource.Id);
            }
        }

        public StaffingResourceManagementViewModel Start()
        {
            SearchPane.Start();
            return this;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Start();
            SearchPane.PropertyChanged += OnSearchPanePropertyChanged;
            SearchPane.Open += OpenStaffingResource;
            ((IActivate)SearchPane).Activate();

            if (_toolbarGroup == null)
            {
                _toolbarGroup = new ToolbarGroup(10)
                                    {
                                        new ToolbarAction(this, "Add", (Func<IEnumerable<IResult>>) Add),
                                        new ToolbarAction(this, "Delete", (Func<IEnumerable<IResult>>) Delete),
                                        new ToolbarAction(this, "Save", (Func<IEnumerable<IResult>>) Save),
                                        new ToolbarAction(this, "Cancel", (Action) Cancel)
                                    };
            }
            _toolbar.AddGroup(_toolbarGroup);
        }

        public void OpenStaffingResource(object sender, EventArgs args)
        {
            var staffingResource = SearchPane.CurrentStaffingResource;

            var form = _forms.GetForm(staffingResource.Id);
            if (form.StaffingResource == null)
                form.Start(staffingResource.Id);
            ActivateItem(form);
        }

        private void OnSearchPanePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentStaffingResource") return;

            NotifyOfPropertyChange(() => CanDelete);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _forms.Clear();
            SearchPane.PropertyChanged -= OnSearchPanePropertyChanged;
            SearchPane.Open -= OpenStaffingResource;
            ((IDeactivate)SearchPane).Deactivate(close);

            _toolbar.RemoveGroup(_toolbarGroup);
        }

        public override void CanClose(Action<bool> callback)
        {
            if (Items.Cast<StaffingResourceDetailViewModel>().Any(d => d.UnitOfWork.HasChanges()))
                CanCloseCore(callback).ToSequentialResult().Execute();
            else
                base.CanClose(callback);
        }

        private IEnumerable<IResult> CanCloseCore(Action<bool> callback)
        {
            var pendingForms =
                Items.Cast<StaffingResourceDetailViewModel>().Where(f => f.UnitOfWork.HasChanges()).ToList();

            foreach (var pendingForm in pendingForms)
            {
                DialogOperationResult<DialogResult> responseOperation;
                yield return responseOperation = _dialogManager.ShowMessage(
                    string.Format("Do you want to save changes you made to {0}?", pendingForm.StaffingResource.FullName),
                    DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel);

                if (responseOperation.DialogResult == DialogResult.Yes)
                {
                    OperationResult commit;
                    using (pendingForm.Busy.GetTicket())
                        yield return commit = pendingForm.UnitOfWork.CommitAsync().ContinueOnError();

                    if (commit.HasError)
                    {
                        _errorHandler.HandleError(commit.Error);
                        yield break;
                    }
                }

                if (responseOperation.DialogResult == DialogResult.No)
                    pendingForm.UnitOfWork.Rollback();
            }

            callback(true);
        }

        public IEnumerable<IResult> Add()
        {
            StaffingResourceNameEditorViewModel nameEditor = _nameEditorFactory.CreatePart();
            yield return _dialogManager.ShowDialog(nameEditor, DialogButtons.OkCancel);

            SearchPane.CurrentStaffingResource = null;

            var form = _forms.NewForm();
            form.Start(nameEditor.FirstName, nameEditor.MiddleName, nameEditor.LastName,
                      vm => _forms.AddForm(vm.StaffingResource.Id, form));
            ActivateItem(form);
        }

        public IEnumerable<IResult> Delete()
        {
            StaffingResourceListItem staffingResource = SearchPane.CurrentStaffingResource;

            yield return _dialogManager.ShowMessage(
                    string.Format("Are you sure you want to delete {0}?", staffingResource.FullName),
                    DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);

            IDomainUnitOfWork unitOfWork = _unitOfWorkManager.Get(staffingResource.Id);

            OperationResult operation;
            using (SearchPane.Busy.GetTicket())
            {
                yield return operation = unitOfWork.StaffingResources.WithIdAsync(
                    staffingResource.Id, result => unitOfWork.StaffingResources.Delete(result)).ContinueOnError();

                if (operation.CompletedSuccessfully)
                    yield return operation = unitOfWork.CommitAsync().ContinueOnError();
            }

            if (operation.CompletedSuccessfully && _forms.FormExists(staffingResource.Id))
                _forms.GetForm(staffingResource.Id).TryClose();

            if (operation.HasError)
                _errorHandler.HandleError(operation.Error);
        }

        public IEnumerable<IResult> Save()
        {
            OperationResult<SaveResult> saveOperation;
            using (ActiveDetail.Busy.GetTicket())
                yield return saveOperation = ActiveUnitOfWork.CommitAsync().ContinueOnError();

            if (saveOperation.HasError)
                _errorHandler.HandleError(saveOperation.Error);
        }

        public void Cancel()
        {
            bool shouldClose = ActiveStaffingResource.EntityFacts.EntityState.IsAdded();
            ActiveUnitOfWork.Rollback();

            if (shouldClose)
                ActiveDetail.TryClose();
        }
    }
}