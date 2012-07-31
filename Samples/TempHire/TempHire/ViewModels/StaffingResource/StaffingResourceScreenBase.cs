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
using Common.Errors;
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace TempHire.ViewModels.StaffingResource
{
    public abstract class StaffingResourceScreenBase : Screen, IDiscoverableViewModel, IHarnessAware
    {
        private bool _isReadOnly;
        private DomainModel.StaffingResource _staffingResource;
        private Guid _staffingResourceId;
        private IResourceMgtUnitOfWork _unitOfWork;

        protected StaffingResourceScreenBase(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                             IErrorHandler errorHandler)
        {
            DomainUnitOfWorkManager = unitOfWorkManager;
            ErrorHandler = errorHandler;
            _isReadOnly = true;
        }

        public IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> DomainUnitOfWorkManager { get; private set; }
        public IErrorHandler ErrorHandler { get; private set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            private set
            {
                _isReadOnly = value;
                _unitOfWork = null;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        protected IResourceMgtUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork ??
                       (_unitOfWork = DomainUnitOfWorkManager.Get(IsReadOnly ? Guid.Empty : _staffingResourceId));
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
            Start(TempHireSampleDataProvider.CreateGuid(1), false);
#endif
        }

        #endregion

        public virtual StaffingResourceScreenBase Start(Guid staffingResourceId, bool readOnly)
        {
            _unitOfWork = null;
            _staffingResourceId = staffingResourceId;
            IsReadOnly = readOnly;
            UnitOfWork.StaffingResources.WithIdAsync(staffingResourceId, result => StaffingResource = result,
                                                     ErrorHandler.HandleError);
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
    }
}