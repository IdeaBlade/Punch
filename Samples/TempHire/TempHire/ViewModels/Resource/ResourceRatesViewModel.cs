using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Repositories;
using DomainModel;

namespace TempHire.ViewModels.Resource
{
    [Export(typeof(IResourceDetailSection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceRatesViewModel : ResourceScreenBase, IResourceDetailSection
    {
        private readonly ExportFactory<RateTypeSelectorViewModel> _rateTypeSelectorFactory;
        private readonly IDialogManager _dialogManager;

        [ImportingConstructor]
        public ResourceRatesViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                      ExportFactory<RateTypeSelectorViewModel> rateTypeSelectorFactory,
                                      IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(repositoryManager, errorHandler)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Rates";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            _rateTypeSelectorFactory = rateTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public bool IsEmpty
        {
            get { return Resource == null || Resource.Rates.Count == 0; }
        }

        public override DomainModel.Resource Resource
        {
            get { return base.Resource; }
            set
            {
                if (base.Resource != null)
                    base.Resource.Rates.CollectionChanged -= RatesCollectionChanged;

                if (value != null)
                    value.Rates.CollectionChanged += RatesCollectionChanged;

                base.Resource = value;
                NotifyOfPropertyChange(() => IsEmpty);
                NotifyOfPropertyChange(() => RatesSorted);
            }
        }

        public IEnumerable<Rate> RatesSorted
        {
            get
            {
                if (Resource != null)
                    return Resource.Rates.OrderBy(r => r.RateType.Sequence);
                return new Rate[0];
            }
        }

        #region IResourceDetailSection Members

        public int Index
        {
            get { return 10; }
        }

        void IResourceDetailSection.Start(Guid resourceId)
        {
            Start(resourceId);
        }

        #endregion

        private void RatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsEmpty);
            NotifyOfPropertyChange(() => RatesSorted);
        }

        public IEnumerable<IResult> Add()
        {
            RateTypeSelectorViewModel rateTypeSelector = _rateTypeSelectorFactory.CreateExport().Value;
            yield return _dialogManager.ShowDialog(rateTypeSelector.Start(Resource.Id), DialogButtons.OkCancel);

            Resource.AddRate(rateTypeSelector.SelectedRateType);
        }

        public void Delete(Rate rate)
        {
            Resource.DeleteRate(rate);
        }
    }
}