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
using Common.BusyWatcher;
using Common.Errors;
using Common.Repositories;
using IdeaBlade.Core;
#if HARNESS
using Common.SampleData;
#endif

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceDetailViewModel : Conductor<IScreen>.Collection.OneActive, IDiscoverableViewModel,
                                           IHarnessAware
    {
        private readonly IDialogManager _dialogManager;
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
                                       IErrorHandler errorHandler, IDialogManager dialogManager,
                                       [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IBusyWatcher busy)
        {
            ResourceSummary = resourceSummary;
            _repositoryManager = repositoryManager;
            _sections = sections.ToList();
            _errorHandler = errorHandler;
            _dialogManager = dialogManager;
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
            Repository.GetResourceAsync(resourceId, OnStartCompleted, _errorHandler.HandleError)
                .OnComplete(args => Busy.RemoveWatch());

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
            _repository.CreateResourceAsync(firstName, middleName, lastName,
                                            resource =>
                                                {
                                                    _repositoryManager.Add(resource.Id, _repository);
                                                    Start(resource.Id);
                                                },
                                            _errorHandler.HandleError)
                .OnComplete(args => Busy.RemoveWatch());

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
                var dialogResult =
                    _dialogManager.ShowMessage("There are unsaved changes. Would you like to continue?",
                                               DialogButtons.YesNo);
                dialogResult.OnComplete(delegate
                                            {
                                                if (dialogResult.DialogResult == DialogResult.Yes)
                                                    Repository.RejectChanges();

                                                callback(dialogResult.DialogResult == DialogResult.Yes);
                                            });
            }
            else
                base.CanClose(callback);
        }
    }
}