using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Repositories;
using DomainModel;
using IdeaBlade.Core;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourcePhoneListViewModel : ResourceScreenBase
    {
        private readonly ExportFactory<PhoneTypeSelectorViewModel> _phoneTypeSelectorFactory;
        private readonly IDialogManager _dialogManager;
        private BindableCollection<ResourcePhoneItemViewModel> _phoneNumbers;

        [ImportingConstructor]
        public ResourcePhoneListViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                          ExportFactory<PhoneTypeSelectorViewModel> phoneTypeSelectorFactory,
                                          IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(repositoryManager, errorHandler)
        {
            _phoneTypeSelectorFactory = phoneTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public override DomainModel.Resource Resource
        {
            get { return base.Resource; }
            set
            {
                if (base.Resource != null)
                    base.Resource.PhoneNumbers.CollectionChanged -= PhoneNumbersCollectionChanged;

                ClearPhoneNumbers();

                if (value != null)
                {
                    PhoneNumbers =
                        new BindableCollection<ResourcePhoneItemViewModel>(
                            value.PhoneNumbers.ToList().Select(p => new ResourcePhoneItemViewModel(p)));
                    value.PhoneNumbers.CollectionChanged += PhoneNumbersCollectionChanged;
                }

                base.Resource = value;
            }
        }

        public BindableCollection<ResourcePhoneItemViewModel> PhoneNumbers
        {
            get { return _phoneNumbers; }
            set
            {
                _phoneNumbers = value;
                NotifyOfPropertyChange(() => PhoneNumbers);
            }
        }

        private void PhoneNumbersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ResourcePhoneItemViewModel item in
                    e.OldItems.Cast<PhoneNumber>().Select(p => PhoneNumbers.First(i => i.Item == p)))
                {
                    PhoneNumbers.Remove(item);
                    item.Dispose();
                }
            }

            if (e.NewItems != null)
                e.NewItems.Cast<PhoneNumber>()
                    .ForEach(p => PhoneNumbers.Add(new ResourcePhoneItemViewModel(p)));

            EnsureDelete();
        }

        private void EnsureDelete()
        {
            PhoneNumbers.ForEach(i => i.NotifyOfPropertyChange(() => i.CanDelete));
        }

        public IEnumerable<IResult> Add()
        {
            PhoneTypeSelectorViewModel phoneTypeSelector = _phoneTypeSelectorFactory.CreateExport().Value;
            yield return _dialogManager.ShowDialog(phoneTypeSelector.Start(Resource.Id), DialogButtons.OkCancel);

            Resource.AddPhoneNumber(phoneTypeSelector.SelectedPhoneType);

            EnsureDelete();
        }

        public void Delete(ResourcePhoneItemViewModel phoneItem)
        {
            Resource.DeletePhoneNumber(phoneItem.Item);

            EnsureDelete();
        }

        public void SetPrimary(ResourcePhoneItemViewModel phoneItem)
        {
            Resource.PrimaryPhoneNumber = phoneItem.Item;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (!close) return;

            ClearPhoneNumbers();
        }

        private void ClearPhoneNumbers()
        {
            if (PhoneNumbers == null) return;

            // Clean up to avoid memory leaks
            PhoneNumbers.ForEach(i => i.Dispose());
            PhoneNumbers.Clear();
        }
    }
}