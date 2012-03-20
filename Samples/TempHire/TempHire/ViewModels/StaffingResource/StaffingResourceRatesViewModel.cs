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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Factories;
using DomainModel;
using DomainServices;

namespace TempHire.ViewModels.StaffingResource
{
    [Export(typeof (IStaffingResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceRatesViewModel : StaffingResourceScreenBase, IStaffingResourceDetailSection
    {
        private readonly IDialogManager _dialogManager;
        private readonly IPartFactory<RateTypeSelectorViewModel> _rateTypeSelectorFactory;

        [ImportingConstructor]
        public StaffingResourceRatesViewModel(IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                              IPartFactory<RateTypeSelectorViewModel> rateTypeSelectorFactory,
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
            get { return StaffingResource == null || StaffingResource.Rates.Count == 0; }
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                    base.StaffingResource.Rates.CollectionChanged -= RatesCollectionChanged;

                if (value != null)
                    value.Rates.CollectionChanged += RatesCollectionChanged;

                base.StaffingResource = value;
                NotifyOfPropertyChange(() => IsEmpty);
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

        void IStaffingResourceDetailSection.Start(Guid staffingResourceId)
        {
            Start(staffingResourceId);
        }

        #endregion

        private void RatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => RatesSorted);
        }

        public IEnumerable<IResult> Add()
        {
            RateTypeSelectorViewModel rateTypeSelector = _rateTypeSelectorFactory.CreatePart();
            yield return _dialogManager.ShowDialog(rateTypeSelector.Start(UnitOfWork), DialogButtons.OkCancel);

            StaffingResource.AddRate(rateTypeSelector.SelectedRateType);
        }

        public void Delete(Rate rate)
        {
            StaffingResource.DeleteRate(rate);
        }
    }
}