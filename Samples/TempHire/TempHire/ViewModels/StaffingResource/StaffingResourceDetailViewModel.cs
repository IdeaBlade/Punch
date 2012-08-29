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
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common;
using Common.Errors;
using DomainServices;
using IdeaBlade.Core;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceDetailViewModel : Conductor<IScreen>.Collection.OneActive, IDiscoverableViewModel,
                                                   IHarnessAware
    {
        private readonly IDialogManager _dialogManager;
        private readonly IErrorHandler _errorHandler;
        private readonly IEnumerable<IStaffingResourceDetailSection> _sections;
        private readonly IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> _unitOfWorkManager;
        private EditMode _editMode;
        private DomainModel.StaffingResource _staffingResource;
        private Guid _staffingResourceId;
        private IResourceMgtUnitOfWork _unitOfWork;

        [ImportingConstructor]
        public StaffingResourceDetailViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                               StaffingResourceSummaryViewModel staffingResourceSummary,
                                               [ImportMany] IEnumerable<IStaffingResourceDetailSection> sections,
                                               IErrorHandler errorHandler, IDialogManager dialogManager)
        {
            StaffingResourceSummary = staffingResourceSummary;
            _unitOfWorkManager = unitOfWorkManager;
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

        public bool IsReadOnly
        {
            get { return EditMode == EditMode.View; }
        }

        public EditMode EditMode
        {
            get { return _editMode; }
            private set
            {
                _editMode = value;
                _unitOfWork = null;
                NotifyOfPropertyChange(() => IsReadOnly);
                NotifyOfPropertyChange(() => EditMode);
            }
        }

        public IResourceMgtUnitOfWork UnitOfWork
        {
            get
            {
                // Return the current sandbox UoW, or if the VM is in view-only mode return the shared UoW associated with Guid.Empty
                var key = EditMode == EditMode.View ? Guid.Empty : _staffingResourceId;
                return _unitOfWork ?? (_unitOfWork = _unitOfWorkManager.Get(key));
            }
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
            Start(TempHireSampleDataProvider.CreateGuid(1), EditMode.Edit);
#endif
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveItem")
                NotifyOfPropertyChange(() => ActiveSectionIndex);
        }

        public StaffingResourceDetailViewModel Start(Guid staffingResourceId, EditMode editMode)
        {
            Busy.AddWatch();

            _unitOfWork = null;
            _staffingResourceId = staffingResourceId;
            EditMode = editMode;
            // Bring resource into cache and defer starting of nested VMs until completed.
            UnitOfWork.StaffingResources.WithIdAsync(staffingResourceId, OnStartCompleted, _errorHandler.HandleError)
                .ContinueWith(op => Busy.RemoveWatch());

            return this;
        }

        private void OnStartCompleted(DomainModel.StaffingResource staffingResource)
        {
            StaffingResource = staffingResource;
            StaffingResourceSummary.Start(staffingResource.Id, EditMode);

            _sections.ForEach(s => s.Start(staffingResource.Id, EditMode));
            if (Items.Count == 0)
            {
                Items.AddRange(_sections.OrderBy(s => s.Index));
                NotifyOfPropertyChange(() => Items);
                ActivateItem(Items.First());
            }
        }

        public StaffingResourceDetailViewModel Start(string firstName, string middleName, string lastName)
        {
            Busy.AddWatch();

            _unitOfWork = _unitOfWorkManager.Create();
            _unitOfWork.StaffingResourceFactory.CreateAsync(null, null)
                .ContinueWith(op =>
                                  {
                                      if (op.CompletedSuccessfully)
                                      {
                                          _unitOfWorkManager.Add(op.Result.Id, _unitOfWork);
                                          op.Result.FirstName = firstName;
                                          op.Result.MiddleName = middleName;
                                          op.Result.LastName = lastName;
                                          Start(op.Result.Id, EditMode.Edit);
                                      }

                                      if (op.HasError)
                                          _errorHandler.HandleError(op.Error);

                                      Busy.RemoveWatch();
                                  });

            return this;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ((IActivate) StaffingResourceSummary).Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ((IDeactivate) StaffingResourceSummary).Deactivate(close);

            if (close)
            {
                StaffingResource = null;
                _unitOfWork = null;
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            if (UnitOfWork.HasChanges())
            {
                Compatibility.ShowMessageAsync(
                    _dialogManager, "There are unsaved changes. Would you like to save your changes?",
                    DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel, null)
                    .ContinueWith(op =>
                                      {
                                          if (op.Result == DialogResult.Yes)
                                          {
                                              Busy.AddWatch();
                                              UnitOfWork.CommitAsync(saveResult => callback(true),
                                                                     _errorHandler.HandleError)
                                                  .ContinueWith(commit => Busy.RemoveWatch());
                                          }

                                          if (op.Result == DialogResult.No)
                                          {
                                              UnitOfWork.Rollback();
                                              callback(true);
                                          }

                                          if (op.Result == DialogResult.Cancel)
                                              callback(false);
                                      });
            }
            else
                base.CanClose(callback);
        }

        public IEnumerable<IResult> RefreshData()
        {
            if (UnitOfWork.HasChanges())
                yield return Compatibility.ShowMessageAsync(
                    _dialogManager,
                    "There are unsaved changes. Refreshing the data will discard all unsaved changes.",
                    DialogButtons.OkCancel, null);

            UnitOfWork.Clear();
            Start(StaffingResource.Id, EditMode);
        }
    }
}