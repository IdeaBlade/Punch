using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using Caliburn.Micro;
using Cocktail;
using Common.Dialog;
using Common.Errors;
using Common.Messages;
using Common.Repositories;
using Common.Toolbar;
using Common.Workspace;
using DomainModel.Projections;
using Action = System.Action;

namespace TempHire.ViewModels.Resource
{
    [Export]
    public class ResourceManagementViewModel : Conductor<IScreen>, IDiscoverableViewModel, IHandle<EntityChangedMessage>,
                                               IWorkspace
    {
        private readonly ExportFactory<ResourceDetailViewModel> _detailFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly ExportFactory<ResourceNameEditorViewModel> _nameEditorFactory;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private readonly DispatcherTimer _selectionChangeTimer;
        private readonly IToolbarManager _toolbar;
        private IScreen _retainedActiveItem;
        private ToolbarGroup _toolbarGroup;

        [ImportingConstructor]
        public ResourceManagementViewModel(ResourceSearchViewModel searchPane,
                                           ExportFactory<ResourceDetailViewModel> detailFactory,
                                           ExportFactory<ResourceNameEditorViewModel> nameEditorFactory,
                                           IRepositoryManager<IResourceRepository> repositoryManager,
                                           IEventAggregator eventAggregator,
                                           IErrorHandler errorHandler,
                                           IToolbarManager toolbar)
        {
            SearchPane = searchPane;
            _detailFactory = detailFactory;
            _nameEditorFactory = nameEditorFactory;
            _repositoryManager = repositoryManager;
            _errorHandler = errorHandler;
            _toolbar = toolbar;

            eventAggregator.Subscribe(this);

            PropertyChanged += OnPropertyChanged;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Resource Management";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            _selectionChangeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 200) };
            _selectionChangeTimer.Tick += OnSelectionChangeElapsed;
        }

        public ResourceSearchViewModel SearchPane { get; private set; }

        public bool CanDelete
        {
            get { return SearchPane.CurrentResource != null; }
        }

        private IResourceRepository ActiveRepository
        {
            get { return _repositoryManager.GetRepository(ActiveResource.Id); }
        }

        private ResourceDetailViewModel ActiveDetail
        {
            get { return ActiveItem as ResourceDetailViewModel; }
        }

        private DomainModel.Resource ActiveResource
        {
            get { return ActiveDetail != null ? ActiveDetail.Resource : null; }
        }

        public bool CanSave
        {
            get
            {
                return ActiveResource != null && ActiveRepository.HasChanges() &&
                       !ActiveResource.EntityFacts.IsDeleted;
            }
        }

        public bool CanCancel
        {
            get { return CanSave; }
        }

        #region IHandle<EntityChangedMessage> Members

        public void Handle(EntityChangedMessage message)
        {
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
                if (_retainedActiveItem != null)
                    _retainedActiveItem.PropertyChanged -= OnActiveDetailPropertyChanged;

                _retainedActiveItem = ActiveItem;
                if (ActiveItem != null)
                    ActiveItem.PropertyChanged += OnActiveDetailPropertyChanged;
            }
        }

        private void OnActiveDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Resource")
            {
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanCancel);
            }
        }

        public ResourceManagementViewModel Start()
        {
            SearchPane.Start();
            return this;
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
                                        new ToolbarAction(this, "Add", (Func<IEnumerable<IResult>>)Add),
                                        new ToolbarAction(this, "Delete", (Func<IEnumerable<IResult>>) Delete),
                                        new ToolbarAction(this, "Save", (Func<IEnumerable<IResult>>) Save),
                                        new ToolbarAction(this, "Cancel", (Action) Cancel)
                                    };
            }
            _toolbar.AddGroup(_toolbarGroup);
        }

        private void OnSelectionChangeElapsed(object sender, EventArgs e)
        {
            _selectionChangeTimer.Stop();

            if (SearchPane.CurrentResource != null)
            {
                Func<ResourceDetailViewModel> target = () => ActiveDetail ?? _detailFactory.CreateExport().Value;
                var result = new NavigateResult<ResourceDetailViewModel>(this, target)
                                 {
                                     Prepare = nav => nav.Target.Start(SearchPane.CurrentResource.Id)
                                 };
                result.Execute(null);
            }

            NotifyOfPropertyChange(() => CanDelete);
        }

        private void OnSearchPanePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentResource") return;

            if (_selectionChangeTimer.IsEnabled) _selectionChangeTimer.Stop();
            _selectionChangeTimer.Start();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
                ActiveItem = null;

            base.OnDeactivate(close);
            SearchPane.PropertyChanged -= OnSearchPanePropertyChanged;
            ((IDeactivate)SearchPane).Deactivate(close);

            _toolbar.RemoveGroup(_toolbarGroup);
        }

        public IEnumerable<IResult> Add()
        {
            ResourceNameEditorViewModel nameEditor = _nameEditorFactory.CreateExport().Value;
            yield return new ShowDialogResult("New Resource", nameEditor);

            SearchPane.CurrentResource = null;

            Func<ResourceDetailViewModel> target = () => ActiveDetail ?? _detailFactory.CreateExport().Value;
            yield return
                new NavigateResult<ResourceDetailViewModel>(this, target)
                    {
                        Prepare = nav =>
                                  nav.Target.Start(nameEditor.FirstName, nameEditor.MiddleName, nameEditor.LastName)
                    };
        }

        public IEnumerable<IResult> Delete()
        {
            ResourceListItem resource = SearchPane.CurrentResource;

            yield return
                new ShowMessageResult("Confirmation",
                                      string.Format("Are you sure you want to delete {0}?", resource.FullName), false);

            using (ActiveDetail.Busy.GetTicket())
            {
                IResourceRepository repository = _repositoryManager.GetRepository(resource.Id);

                bool success = false;
                yield return CoroutineFns.AsResult(
                    () => repository.DeleteResourceAsync(resource.Id, () => success = true, _errorHandler.HandleError));

                if (success)
                {
                    // Rerun the search
                    SearchPane.Search();

                    if (ActiveResource != null && ActiveResource.Id == resource.Id)
                        ActiveItem.TryClose();
                }
            }
        }

        public IEnumerable<IResult> Save()
        {
            using (ActiveDetail.Busy.GetTicket())
            {
                yield return CoroutineFns.AsResult(() => ActiveRepository.SaveAsync(onFail: _errorHandler.HandleError));

                SearchPane.Search(ActiveResource.Id);

                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanCancel);
            }
        }

        public void Cancel()
        {
            var shouldClose = ActiveResource.EntityFacts.IsAdded;
            ActiveRepository.RejectChanges();

            if (shouldClose)
                ActiveDetail.TryClose();
        }
    }
}