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
using Caliburn.Micro;
using Cocktail;
using Common;
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace TempHire.ViewModels.StaffingResource
{
    public abstract class StaffingResourceScreenBase : Screen, IDiscoverableViewModel, IHarnessAware
    {
        private EditMode _editMode;
        private DomainModel.StaffingResource _staffingResource;
        private Guid _staffingResourceId;
        private IResourceMgtUnitOfWork _unitOfWork;

        protected StaffingResourceScreenBase(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager)
        {
            UnitOfWorkManager = unitOfWorkManager;
            EditMode = EditMode.View;
        }

        public IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> UnitOfWorkManager { get; private set; }

        public EditMode EditMode
        {
            get { return _editMode; }
            private set
            {
                _editMode = value;
                _unitOfWork = null;
                NotifyOfPropertyChange(() => EditMode);
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        public bool IsReadOnly
        {
            get { return EditMode == EditMode.View; }
        }

        protected IResourceMgtUnitOfWork UnitOfWork
        {
            get
            {
                // Return the current sandbox UoW, or if the VM is in view-only mode return the shared UoW associated with Guid.Empty
                var key = EditMode == EditMode.View ? Guid.Empty : _staffingResourceId;
                return _unitOfWork ?? (_unitOfWork = UnitOfWorkManager.Get(key));
            }
        }

        public virtual DomainModel.StaffingResource StaffingResource
        {
            get { return _staffingResource; }
            set
            {
                _staffingResource = value;
                NotifyOfPropertyChange(() => StaffingResource);
            }
        }

        #region IHarnessAware Members

        public void Setup()
        {
#if HARNESS
            Start(TempHireSampleDataProvider.CreateGuid(1), EditMode.Edit);
#endif
        }

        #endregion

        public virtual StaffingResourceScreenBase Start(Guid staffingResourceId, EditMode editMode)
        {
            LoadDataAsync(staffingResourceId, editMode);
            return this;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
            {
                StaffingResource = null;
                _unitOfWork = null;
            }
        }

        private async void LoadDataAsync(Guid staffingResourceId, EditMode editMode)
        {
            _unitOfWork = null;
            _staffingResourceId = staffingResourceId;
            EditMode = editMode;
            StaffingResource = await UnitOfWork.StaffingResources.WithIdAsync(staffingResourceId);
        }
    }
}