using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.BusyWatcher;
using Common.Dialog;
using Common.Errors;
using DomainModel;
using DomainModel.Repositories;
using IdeaBlade.Application.Framework.Core.ViewModel;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using TempHire.Repositories;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceDetailViewModel : Conductor<IScreen>.Collection.OneActive, IDiscoverableViewModel,
                                           IHarnessAware
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IResourceRepository> _repositoryManager;
        private readonly IEnumerable<IResourceDetailSection> _sections;
        private IResourceRepository _repository;
        private DomainModel.Resource _resource;
        private Guid _resourceId;

        [ImportingConstructor]
        public ResourceDetailViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                       ResourceSummaryViewModel resourceSummary,
                                       [ImportMany] IEnumerable<IResourceDetailSection> sections,
                                       IErrorHandler errorHandler,
                                       [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IBusyWatcher busy)
        {
            ResourceSummary = resourceSummary;
            _repositoryManager = repositoryManager;
            _sections = sections.ToList();
            _errorHandler = errorHandler;
            Busy = busy;

            PropertyChanged += OnPropertyChanged;
        }

        public ResourceSummaryViewModel ResourceSummary { get; private set; }

        public IBusyWatcher Busy { get; private set; }

        public bool Visible
        {
            get { return Resource != null; }
        }

        private IResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_resourceId)); }
        }

        public int ActiveSectionIndex
        {
            get { return Items.IndexOf(ActiveItem); }
            set { ActivateItem(Items[Math.Max(value, 0)]); }
        }

        public DomainModel.Resource Resource
        {
            get { return _resource; }
            set
            {
                _resource = value;
                NotifyOfPropertyChange(() => Resource);
                NotifyOfPropertyChange(() => Visible);
            }
        }

        #region IHarnessAware Members

        public void Setup()
        {
#if HARNESS
            //Start("John", "M.", "Doe");
            Start(TempHireSampleDataProvider.CreateGuid(1));
#endif
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveItem")
                NotifyOfPropertyChange(() => ActiveSectionIndex);
        }

        public ResourceDetailViewModel Start(Guid resourceId)
        {
            Busy.AddWatch();

            _repository = null;
            _resourceId = resourceId;
            // Bring resource into cache and defer starting of nested VMs until completed.
            INotifyCompleted op = Repository.GetResourceAsync(resourceId, OnStartCompleted, _errorHandler.HandleError);
            op.WhenCompleted(e => Busy.RemoveWatch());

            return this;
        }

        private void OnStartCompleted(DomainModel.Resource resource)
        {
            Resource = resource;
            ResourceSummary.Start(resource.Id);

            _sections.ForEach(s => s.Start(resource.Id));
            if (Items.Count == 0)
            {
                Items.AddRange(_sections.OrderBy(s => s.Index).Cast<IScreen>());
                NotifyOfPropertyChange(() => Items);
                ActivateItem(Items.First());
            }
        }

        public ResourceDetailViewModel Start(string firstName, string middleName, string lastName)
        {
            Busy.AddWatch();

            _repository = _repositoryManager.Create();
            INotifyCompleted op = _repository
                .CreateResourceAsync(firstName, middleName, lastName,
                                     resource =>
                                         {
                                             _repositoryManager.Add(resource.Id, _repository);
                                             Start(resource.Id);
                                         },
                                     _errorHandler.HandleError);
            op.WhenCompleted(e => Busy.RemoveWatch());

            return this;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ((IActivate) ResourceSummary).Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ((IDeactivate) ResourceSummary).Deactivate(close);

            if (close)
            {
                Resource = null;
                _repository = null;
                Items.Clear();
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (Repository.HasChanges())
            {
                var result = new ShowMessageResult("Confirmation",
                                                   "There are unsaved changes. Would you like to continue?", false);
                result.Completed += (sender, args) =>
                                        {
                                            if (!args.WasCancelled)
                                                Repository.RejectChanges();

                                            callback(!args.WasCancelled);
                                        };
                result.Execute(null);
            }
            else
                base.CanClose(callback);
        }

    }
}