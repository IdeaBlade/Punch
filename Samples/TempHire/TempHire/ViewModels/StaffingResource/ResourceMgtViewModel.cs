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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using Cocktail;
using Common;
using Common.Messages;
using Common.Toolbar;
using DomainServices;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export]
    public class ResourceMgtViewModel : Conductor<IScreen>, IDiscoverableViewModel,
                                        IHandle<EntityChangedMessage>
    {
        private readonly IDialogManager _dialogManager;
        private readonly ExportFactory<StaffingResourceNameEditorViewModel> _nameEditorFactory;
        private readonly INavigator _navigator;
        private readonly DispatcherTimer _selectionChangeTimer;
        private readonly IToolbarManager _toolbar;
        private IScreen _retainedActiveItem;
        private ToolbarGroup _toolbarGroup;

        [ImportingConstructor]
        public ResourceMgtViewModel(StaffingResourceSearchViewModel searchPane,
                                    ExportFactory<StaffingResourceNameEditorViewModel> nameEditorFactory,
                                    IDialogManager dialogManager, IToolbarManager toolbar)
        {
            SearchPane = searchPane;
            _nameEditorFactory = nameEditorFactory;
            _dialogManager = dialogManager;
            _toolbar = toolbar;
            _navigator = new Navigator(this);

            PropertyChanged += OnPropertyChanged;

            _selectionChangeTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 200)};
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

        public async void Add()
        {
            try
            {
                var nameEditor = _nameEditorFactory.CreateExport().Value;
                await _dialogManager.ShowDialogAsync(nameEditor, DialogButtons.OkCancel);

                SearchPane.CurrentStaffingResource = null;

                await _navigator.NavigateToAsync<StaffingResourceDetailViewModel>(
                    target => target.Start(nameEditor.FirstName, nameEditor.MiddleName, nameEditor.LastName));
            }
            catch (TaskCanceledException)
            {
                UpdateCommands();
            }
        }

        public async void Delete()
        {
            await _dialogManager.ShowMessageAsync(
                string.Format("Are you sure you want to delete {0}?", ActiveStaffingResource.FullName),
                DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);

            try
            {
                using (ActiveDetail.Busy.GetTicket())
                {
                    ActiveUnitOfWork.StaffingResources.Delete(ActiveStaffingResource);
                    await ActiveUnitOfWork.CommitAsync();
                }

                ActiveItem.TryClose();
            }
            catch (TaskCanceledException)
            {
                ActiveUnitOfWork.Rollback();
            }
            catch (Exception)
            {
                ActiveUnitOfWork.Rollback();
                throw;
            }
        }

        public void Edit()
        {
            ActiveDetail.Start(ActiveStaffingResource.Id, EditMode.Edit);
        }

        public async void Save()
        {
            using (ActiveDetail.Busy.GetTicket())
                await ActiveUnitOfWork.CommitAsync();

            ActiveDetail.Start(ActiveStaffingResource.Id, EditMode.View);
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

        public void RefreshData()
        {
            ActiveDetail.RefreshData();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Start();
            SearchPane.PropertyChanged += OnSearchPanePropertyChanged;
            ((IActivate) SearchPane).Activate();

            if (_toolbarGroup == null)
            {
                _toolbarGroup = new ToolbarGroup(10)
                                    {
                                        new ToolbarAction(this, "Add", Add),
                                        new ToolbarAction(this, "Delete", Delete),
                                        new ToolbarAction(this, "Edit", Edit),
                                        new ToolbarAction(this, "Save", Save),
                                        new ToolbarAction(this, "Cancel", Cancel),
                                        new ToolbarAction(this, "Refresh", RefreshData)
                                    };
            }
            _toolbar.AddGroup(_toolbarGroup);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SearchPane.PropertyChanged -= OnSearchPanePropertyChanged;
            ((IDeactivate) SearchPane).Deactivate(close);

            _toolbar.RemoveGroup(_toolbarGroup);
        }

        private async void OnSelectionChangeElapsed(object sender, EventArgs e)
        {
            _selectionChangeTimer.Stop();
            if (SearchPane.CurrentStaffingResource == null) return;

            try
            {
                await _navigator.NavigateToAsync<StaffingResourceDetailViewModel>(
                    target => target.Start(SearchPane.CurrentStaffingResource.Id, EditMode.View));
            }
            catch (TaskCanceledException)
            {
                UpdateCommands();
            }
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