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
using Common.Repositories;
using IdeaBlade.Core;

#if HARNESS
using Common.SampleData;
#endif

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceDetailViewModel : Conductor<IScreen>.Collection.OneActive, IDiscoverableViewModel,
                                                   IHarnessAware
    {
        private readonly IDialogManager _dialogManager;
        private readonly IErrorHandler _errorHandler;
        private readonly IRepositoryManager<IStaffingResourceRepository> _repositoryManager;
        private readonly IEnumerable<IStaffingResourceDetailSection> _sections;
        private IStaffingResourceRepository _repository;
        private DomainModel.StaffingResource _staffingResource;
        private Guid _staffingResourceId;

        [ImportingConstructor]
        public StaffingResourceDetailViewModel(IRepositoryManager<IStaffingResourceRepository> repositoryManager,
                                               StaffingResourceSummaryViewModel staffingResourceSummary,
                                               [ImportMany] IEnumerable<IStaffingResourceDetailSection> sections,
                                               IErrorHandler errorHandler, IDialogManager dialogManager)
        {
            StaffingResourceSummary = staffingResourceSummary;
            _repositoryManager = repositoryManager;
            _sections = sections.ToList();
            _errorHandler = errorHandler;
            _dialogManager = dialogManager;
            Busy = new BusyWatcher();

            PropertyChanged += OnPropertyChanged;
        }

        public StaffingResourceSummaryViewModel StaffingResourceSummary { get; private set; }

        public IBusyWatcher Busy { get; private set; }

        public bool Visible
        {
            get { return StaffingResource != null; }
        }

        private IStaffingResourceRepository Repository
        {
            get { return _repository ?? (_repository = _repositoryManager.GetRepository(_staffingResourceId)); }
        }

        public int ActiveSectionIndex
        {
            get { return Items.IndexOf(ActiveItem); }
            set { ActivateItem(Items[Math.Max(value, 0)]); }
        }

        public DomainModel.StaffingResource StaffingResource
        {
            get { return _staffingResource; }
            set
            {
                _staffingResource = value;
                NotifyOfPropertyChange(() => StaffingResource);
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

        public StaffingResourceDetailViewModel Start(Guid staffingResourceId)
        {
            Busy.AddWatch();

            _repository = null;
            _staffingResourceId = staffingResourceId;
            // Bring resource into cache and defer starting of nested VMs until completed.
            Repository.GetStaffingResourceAsync(staffingResourceId, OnStartCompleted, _errorHandler.HandleError)
                .OnComplete(args => Busy.RemoveWatch());

            return this;
        }

        private void OnStartCompleted(DomainModel.StaffingResource staffingResource)
        {
            StaffingResource = staffingResource;
            StaffingResourceSummary.Start(staffingResource.Id);

            _sections.ForEach(s => s.Start(staffingResource.Id));
            if (Items.Count == 0)
            {
                Items.AddRange(_sections.OrderBy(s => s.Index).Cast<IScreen>());
                NotifyOfPropertyChange(() => Items);
                ActivateItem(Items.First());
            }
        }

        public StaffingResourceDetailViewModel Start(string firstName, string middleName, string lastName)
        {
            Busy.AddWatch();

            _repository = _repositoryManager.Create();
            _repository.CreateStaffingResourceAsync(firstName, middleName, lastName,
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
            ((IActivate)StaffingResourceSummary).Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ((IDeactivate)StaffingResourceSummary).Deactivate(close);

            if (close)
            {
                StaffingResource = null;
                _repository = null;
                Items.Clear();
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (Repository.HasChanges())
            {
                DialogOperationResult<DialogResult> dialogResult =
                    _dialogManager.ShowMessage("There are unsaved changes. Would you like to save your changes?",
                                               DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel);
                dialogResult.OnComplete(delegate
                                            {
                                                if (dialogResult.DialogResult == DialogResult.Yes)
                                                {
                                                    Busy.AddWatch();
                                                    Repository.SaveAsync(() => callback(true), _errorHandler.HandleError)
                                                        .OnComplete(args => Busy.RemoveWatch());
                                                }

                                                if (dialogResult.DialogResult == DialogResult.No)
                                                {
                                                    Repository.RejectChanges();
                                                    callback(true);
                                                }

                                                if (dialogResult.DialogResult == DialogResult.Cancel)
                                                    callback(false);
                                            });
            }
            else
                base.CanClose(callback);
        }
    }

    [Export(typeof(IPartFactory<StaffingResourceDetailViewModel>))]
    public class StaffingResourceDetailViewModelFactory : PartFactoryBase<StaffingResourceDetailViewModel>
    {
    }
}