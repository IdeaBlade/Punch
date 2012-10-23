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
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common;
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
                                               IDialogManager dialogManager)
        {
            StaffingResourceSummary = staffingResourceSummary;
            _unitOfWorkManager = unitOfWorkManager;
            _sections = sections.ToList();
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
            LoadDataAsync(staffingResourceId, editMode);
            return this;
        }

        private async void LoadDataAsync(Guid staffingResourceId, EditMode editMode)
        {
            using (Busy.GetTicket())
            {
                _unitOfWork = null;
                _staffingResourceId = staffingResourceId;
                EditMode = editMode;

                StaffingResource = await UnitOfWork.StaffingResources.WithIdAsync(staffingResourceId);
                StaffingResourceSummary.Start(StaffingResource.Id, EditMode);
                _sections.ForEach(s => s.Start(StaffingResource.Id, EditMode));
                if (Items.Count == 0)
                {
                    Items.AddRange(_sections.OrderBy(s => s.Index));
                    NotifyOfPropertyChange(() => Items);
                    ActivateItem(Items.First());
                }
            }
        }

        public StaffingResourceDetailViewModel Start(string firstName, string middleName, string lastName)
        {
            LoadDataAsync(firstName, middleName, lastName);
            return this;
        }

        private async void LoadDataAsync(string firstName, string middleName, string lastName)
        {
            using (Busy.GetTicket())
            {
                _unitOfWork = _unitOfWorkManager.Create();
                var staffingResource = await _unitOfWork.StaffingResourceFactory.CreateAsync();

                _unitOfWorkManager.Add(staffingResource.Id, _unitOfWork);
                staffingResource.FirstName = firstName;
                staffingResource.MiddleName = middleName;
                staffingResource.LastName = lastName;
                Start(staffingResource.Id, EditMode.Edit);
            }
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

        public override async void CanClose(Action<bool> callback)
        {
            try
            {
                if (UnitOfWork.HasChanges())
                {
                    var dialogResult = await _dialogManager.ShowMessageAsync(
                        "There are unsaved changes. Would you like to save your changes?",
                        DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel);


                    using (Busy.GetTicket())
                    {
                        if (dialogResult == DialogResult.Yes)
                            await UnitOfWork.CommitAsync();

                        if (dialogResult == DialogResult.No)
                            UnitOfWork.Rollback();

                        callback(true);
                    }
                }
                else
                    base.CanClose(callback);
            }
            catch (TaskCanceledException)
            {
                callback(false);
            }
            catch (Exception)
            {
                callback(false);
                throw;
            }
        }

        public async void RefreshData()
        {
            if (UnitOfWork.HasChanges())
                await _dialogManager.ShowMessageAsync(
                    "There are unsaved changes. Refreshing the data will discard all unsaved changes.",
                    DialogButtons.OkCancel);

            UnitOfWork.Clear();
            Start(StaffingResource.Id, EditMode);
        }
    }
}