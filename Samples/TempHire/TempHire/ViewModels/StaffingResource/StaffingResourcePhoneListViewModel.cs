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
using IdeaBlade.Core;
using IdeaBlade.Linq;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourcePhoneListViewModel : StaffingResourceScreenBase
    {
        private readonly IDialogManager _dialogManager;
        private readonly IPartFactory<ItemSelectorViewModel> _phoneTypeSelectorFactory;
        private BindableCollection<StaffingResourcePhoneItemViewModel> _phoneNumbers;

        [ImportingConstructor]
        public StaffingResourcePhoneListViewModel(IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                                  IPartFactory<ItemSelectorViewModel> phoneTypeSelectorFactory,
                                                  IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(unitOfWorkManager, errorHandler)
        {
            _phoneTypeSelectorFactory = phoneTypeSelectorFactory;
            _dialogManager = dialogManager;
        }

        public override DomainModel.StaffingResource StaffingResource
        {
            get { return base.StaffingResource; }
            set
            {
                if (base.StaffingResource != null)
                    base.StaffingResource.PhoneNumbers.CollectionChanged -= PhoneNumbersCollectionChanged;

                ClearPhoneNumbers();

                if (value != null)
                {
                    PhoneNumbers =
                        new BindableCollection<StaffingResourcePhoneItemViewModel>(
                            value.PhoneNumbers.ToList().Select(p => new StaffingResourcePhoneItemViewModel(p)));
                    value.PhoneNumbers.CollectionChanged += PhoneNumbersCollectionChanged;
                }

                base.StaffingResource = value;
            }
        }

        public BindableCollection<StaffingResourcePhoneItemViewModel> PhoneNumbers
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
                foreach (StaffingResourcePhoneItemViewModel item in
                    e.OldItems.Cast<PhoneNumber>().Select(p => PhoneNumbers.First(i => i.Item == p)))
                {
                    PhoneNumbers.Remove(item);
                    item.Dispose();
                }
            }

            if (e.NewItems != null)
                e.NewItems.Cast<PhoneNumber>()
                    .ForEach(p => PhoneNumbers.Add(new StaffingResourcePhoneItemViewModel(p)));

            EnsureDelete();
        }

        private void EnsureDelete()
        {
            PhoneNumbers.ForEach(i => i.NotifyOfPropertyChange(() => i.CanDelete));
        }

        public IEnumerable<IResult> Add()
        {
            var phoneTypes = UnitOfWork.PhoneNumberTypes;
            var phoneTypeSelector = _phoneTypeSelectorFactory.CreatePart()
                .Start("Select type:", "Name",
                       () => phoneTypes.FindAsync(orderBy: q => q.OrderBy(t => t.Name),
                                                  onFail: ErrorHandler.HandleError));

            yield return _dialogManager.ShowDialogAsync(phoneTypeSelector, DialogButtons.OkCancel);

            StaffingResource.AddPhoneNumber((PhoneNumberType) phoneTypeSelector.SelectedItem);

            EnsureDelete();
        }

        public void Delete(StaffingResourcePhoneItemViewModel phoneItem)
        {
            StaffingResource.DeletePhoneNumber(phoneItem.Item);

            EnsureDelete();
        }

        public void SetPrimary(StaffingResourcePhoneItemViewModel phoneItem)
        {
            StaffingResource.PrimaryPhoneNumber = phoneItem.Item;
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