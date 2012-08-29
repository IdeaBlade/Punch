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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common;
using Common.Errors;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof(IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceRatesViewModel : StaffingResourceScreenBase, IStaffingResourceDetailSection
    {
        private readonly IDialogManager _dialogManager;
        private readonly ExportFactory<ItemSelectorViewModel> _rateTypeSelectorFactory;

        [ImportingConstructor]
        public StaffingResourceRatesViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                              ExportFactory<ItemSelectorViewModel> rateTypeSelectorFactory,
                                              IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(unitOfWorkManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Rates";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            _rateTypeSelectorFactory = rateTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public bool IsEmpty
        {
            get
            {
                return StaffingResource != null && !StaffingResource.Rates.IsPendingEntityList &&
                       StaffingResource.Rates.Count == 0;
            }
        }

        public bool IsPending
        {
            get { return StaffingResource == null || StaffingResource.Rates.IsPendingEntityList; }
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                {
                    base.StaffingResource.Rates.CollectionChanged -= RatesCollectionChanged;
                    base.StaffingResource.Rates.PendingEntityListResolved -= RatesOnPendingEntityListResolved;
                }

                if (value != null)
                {
                    value.Rates.CollectionChanged += RatesCollectionChanged;
                    value.Rates.PendingEntityListResolved += RatesOnPendingEntityListResolved;
                }

                base.StaffingResource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => IsPending);
                NotifyOfPropertyChange(() => RatesSorted);
            }
        }

        public IEnumerable<Rate> RatesSorted
        {
            get
            {
                if (StaffingResource != null)
                    return StaffingResource.Rates.OrderBy(r => r.RateType.Sequence);
                return new Rate[0];
            }
        }

        #region IStaffingResourceDetailSection Members

        public int Index
        {
            get { return 10; }
        }

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId, EditMode editMode)
        {
            Start(staffingResourceId, editMode);
        }

        #endregion

        public IEnumerable<IResult> Add()
        {
            var rateTypes = UnitOfWork.RateTypes;
            var rateTypeSelector = _rateTypeSelectorFactory.CreateExport().Value
                .Start("Select type:", "DisplayName",
                       () =>
                       rateTypes.AllAsync(q => q.OrderBy(t => t.DisplayName), null, null, ErrorHandler.HandleError));

            yield return Compatibility.ShowDialogAsync(_dialogManager, rateTypeSelector, DialogButtons.OkCancel, null);

            StaffingResource.AddRate((RateType) rateTypeSelector.SelectedItem);
        }

        public void Delete(Rate rate)
        {
            StaffingResource.DeleteRate(rate);
        }

        private void RatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => RatesSorted);
        }

        private void RatesOnPendingEntityListResolved(
            object sender, PendingEntityListResolvedEventArgs<Rate> pendingEntityListResolvedEventArgs)
        {
            NotifyOfPropertyChange(() => IsPending);
            NotifyOfPropertyChange(() => IsEmpty);
        }
    }
}