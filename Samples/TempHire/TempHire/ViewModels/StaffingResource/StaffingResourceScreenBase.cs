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
        private DomainModel.StaffingResource _staffingResource;
        private Guid _staffingResourceId;
        private IDomainUnitOfWork _unitOfWork;

        protected StaffingResourceScreenBase(IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                             IErrorHandler errorHandler)
        {
            DomainUnitOfWorkManager = unitOfWorkManager;
            ErrorHandler = errorHandler;
        }

        public IDomainUnitOfWorkManager<IDomainUnitOfWork> DomainUnitOfWorkManager { get; private set; }
        public IErrorHandler ErrorHandler { get; private set; }

        protected IDomainUnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (_unitOfWork = DomainUnitOfWorkManager.Get(_staffingResourceId)); }
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
            Start(TempHireSampleDataProvider.CreateGuid(1));
#endif
        }

        #endregion

        public virtual StaffingResourceScreenBase Start(Guid staffingResourceId)
        {
            _unitOfWork = null;
            _staffingResourceId = staffingResourceId;
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