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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Caliburn.Micro;
using Cocktail;
using Common;
using Common.Errors;
using Common.Factories;
using Common.Messages;
using Common.Toolbar;
using Common.Workspace;
using DomainServices;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace TempHire.ViewModels.StaffingResource
{
    public class ResourceMgtWorkspace : LazyWorkspace<ResourceMgtViewModel>
    {
        public ResourceMgtWorkspace()
            : base("Resource Management", false, 10)
        {
        }
    }

    [Export]
    public class ResourceMgtViewModel : Conductor<IScreen>, IDiscoverableViewModel,
                                        IHandle<EntityChangedMessage>
    {
        private readonly IPartFactory<StaffingResourceDetailViewModel> _detailFactory;
        private readonly IDialogManager _dialogManager;
        private readonly IErrorHandler _errorHandler;
        private readonly IPartFactory<StaffingResourceNameEditorViewModel> _nameEditorFactory;
        private readonly NavigationService<StaffingResourceDetailViewModel> _navigationService;
        private readonly DispatcherTimer _selectionChangeTimer;
        private readonly IToolbarManager _toolbar;
        private IScreen _retainedActiveItem;
        private ToolbarGroup _toolbarGroup;

        [ImportingConstructor]
        public ResourceMgtViewModel(StaffingResourceSearchViewModel searchPane,
                                    IPartFactory<StaffingResourceDetailViewModel> detailFactory,
                                    IPartFactory<StaffingResourceNameEditorViewModel> nameEditorFactory,
                                    IErrorHandler errorHandler, IDialogManager dialogManager,
                                    IToolbarManager toolbar)
        {
            SearchPane = searchPane;
            _detailFactory = detailFactory;
            _nameEditorFactory = nameEditorFactory;
            _errorHandler = errorHandler;
            _dialogManager = dialogManager;
            _toolbar = toolbar;
            _navigationService = new NavigationService<StaffingResourceDetailViewModel>(this);

            PropertyChanged += OnPropertyChanged;

            _selectionChangeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 200) };
            _selectionChangeTimer.Tick += OnSelectionChangeElapsed;
        }

        public StaffingResourceSearchViewModel SearchPane { get; private set; }

        public bool CanAdd
        {
            get { return ActiveDetail == null || ActiveDetail.IsReadOnly; }
        }

        public bool CanDelete
        {
            get { return ActiveStaffingResource != null && ActiveDetail.IsReadOnly; }
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
            get { return ActiveDetail != null && !ActiveDetail.IsReadOnly; }
        }

        public bool CanRefreshData
        {
            get { return ActiveStaffingResource != null && !ActiveStaffingResource.EntityFacts.EntityState.IsAdded(); }
        }

        public bool CanEdit
        {
            get { return ActiveStaffingResource != null && ActiveDetail.IsReadOnly; }
        }

        private IResourceMgtUnitOfWork ActiveUnitOfWork
        {
            get { return ActiveDetail != null ? ActiveDetail.UnitOfWork : null; }
        }

        private StaffingResourceDetailViewModel ActiveDetail
        {
            get { return ActiveItem as StaffingResourceDetailViewModel; }
        }

        private DomainModel.StaffingResource ActiveStaffingResource
        {
            get { return ActiveDetail != null ? ActiveDetail.StaffingResource : null; }
        }

        #region IHandle<EntityChangedMessage> Members

        public void Handle(EntityChangedMessage message)
        {
            if (ActiveStaffingResource == null || !ActiveUnitOfWork.HasEntity(message.Entity))
                return;

            UpdateCommands();
        }

        #endregion

        public ResourceMgtViewModel Start()
        {
            SearchPane.Start();
            return this;
        }

        public IEnumerable<IResult> Add()
        {
            var nameEditor = _nameEditorFactory.CreatePart();
            yield return _dialogManager.ShowDialogAsync(nameEditor, DialogButtons.OkCancel);

            SearchPane.CurrentStaffingResource = null;

            _navigationService.NavigateToAsync(
                () => ActiveDetail ?? _detailFactory.CreatePart(),
                target => target.Start(nameEditor.FirstName, nameEditor.MiddleName, nameEditor.LastName))
                .ContinueWith(navigation => { if (navigation.Cancelled) UpdateCommands(); });
        }

        public IEnumerable<IResult> Delete()
        {
            yield return _dialogManager.ShowMessageAsync(
                string.Format("Are you sure you want to delete {0}?", ActiveStaffingResource.FullName),
                DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);

            OperationResult operation;
            using (ActiveDetail.Busy.GetTicket())
            {
                ActiveUnitOfWork.StaffingResources.Delete(ActiveStaffingResource);
                yield return operation = ActiveUnitOfWork
                                             .CommitAsync()
                                             .ContinueWith(op =>
                                                               {
                                                                   if (!op.CompletedSuccessfully)
                                                                       ActiveUnitOfWork.Rollback();
                                                               });
            }

            if (operation.CompletedSuccessfully)
                ActiveItem.TryClose();

            if (operation.HasError)
                _errorHandler.HandleError(operation.Error);
        }

        public void Edit()
        {
            ActiveDetail.Start(ActiveStaffingResource.Id, EditMode.Edit);
        }

        public IEnumerable<IResult> Save()
        {
            OperationResult<bool> checkDuplicates;
            yield return checkDuplicates = ActiveUnitOfWork.Validation.CheckIfDuplicateAsync(
                ActiveStaffingResource, onFail: _errorHandler.HandleError);
            if (checkDuplicates.CompletedSuccessfully && checkDuplicates.Result)
            {
                yield return
                    _dialogManager.ShowMessageAsync("A resource with the same name already exists.", DialogButtons.Ok);
                yield break;
            }

            if (checkDuplicates.HasError)
                yield break;

            OperationResult<SaveResult> saveOperation;
            using (ActiveDetail.Busy.GetTicket())
                yield return saveOperation = ActiveUnitOfWork.CommitAsync().ContinueOnError();

            if (saveOperation.CompletedSuccessfully)
                ActiveDetail.Start(ActiveStaffingResource.Id, EditMode.View);

            if (saveOperation.HasError)
                _errorHandler.HandleError(saveOperation.Error);
        }

        public void Cancel()
        {
            var shouldClose = ActiveStaffingResource.EntityFacts.EntityState.IsAdded();
            ActiveUnitOfWork.Rollback();

            if (shouldClose)
                ActiveDetail.TryClose();
            else
                ActiveDetail.Start(ActiveStaffingResource.Id, EditMode.View);
        }

        public IEnumerable<IResult> RefreshData()
        {
            return ActiveDetail.RefreshData();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Start();
            SearchPane.PropertyChanged += OnSearchPanePropertyChanged;
            ((IActivate)SearchPane).Activate();

            if (_toolbarGroup == null)
            {
                _toolbarGroup = new ToolbarGroup(10)
                                    {
                                        new ToolbarAction(this, "Add", (Func<IEnumerable<IResult>>) Add),
                                        new ToolbarAction(this, "Delete", (Func<IEnumerable<IResult>>) Delete),
                                        new ToolbarAction(this, "Edit", (Action) Edit),
                                        new ToolbarAction(this, "Save", (Func<IEnumerable<IResult>>) Save),
                                        new ToolbarAction(this, "Cancel", (Action) Cancel),
                                        new ToolbarAction(this, "Refresh", (Func<IEnumerable<IResult>>) RefreshData)
                                    };
            }
            _toolbar.AddGroup(_toolbarGroup);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SearchPane.PropertyChanged -= OnSearchPanePropertyChanged;
            ((IDeactivate)SearchPane).Deactivate(close);

            _toolbar.RemoveGroup(_toolbarGroup);
        }

        private void OnSelectionChangeElapsed(object sender, EventArgs e)
        {
            _selectionChangeTimer.Stop();
            if (SearchPane.CurrentStaffingResource == null) return;

            _navigationService.NavigateToAsync(() => ActiveDetail ?? _detailFactory.CreatePart(),
                                               target => target.Start(SearchPane.CurrentStaffingResource.Id, EditMode.View))
                .ContinueWith(navigation => { if (navigation.Cancelled) UpdateCommands(); });
        }

        private void OnSearchPanePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentStaffingResource") return;

            if (_selectionChangeTimer.IsEnabled) _selectionChangeTimer.Stop();
            _selectionChangeTimer.Start();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ActiveItem") return;

            if (_retainedActiveItem != null)
                _retainedActiveItem.PropertyChanged -= OnActiveDetailPropertyChanged;

            _retainedActiveItem = ActiveItem;
            if (ActiveItem != null)
                ActiveItem.PropertyChanged += OnActiveDetailPropertyChanged;

            UpdateCommands();
        }

        private void OnActiveDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StaffingResource")
                UpdateCommands();
        }

        private void UpdateCommands()
        {
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanCancel);
            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => CanRefreshData);
            NotifyOfPropertyChange(() => CanEdit);
            NotifyOfPropertyChange(() => CanAdd);
        }
    }
}