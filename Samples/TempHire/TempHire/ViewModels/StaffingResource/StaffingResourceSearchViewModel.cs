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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Messages;
using DomainModel.Projections;
using DomainServices;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceSearchViewModel : Screen, IDiscoverableViewModel, IHarnessAware, IHandle<SavedMessage>
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> _unitOfWorkManager;
        private StaffingResourceListItem _currentStaffingResource;
        private BindableCollection<StaffingResourceListItem> _items;

        private string _searchText;
        private IResourceMgtUnitOfWork _unitOfWork;

        [ImportingConstructor]
        public StaffingResourceSearchViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                               IErrorHandler errorHandler)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _errorHandler = errorHandler;
            Busy = new BusyWatcher();
        }

        public IBusyWatcher Busy { get; private set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
                NotifyOfPropertyChange(() => CanSearch);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public BindableCollection<StaffingResourceListItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public bool CanSearch
        {
            get { return !string.IsNullOrWhiteSpace(SearchText); }
        }

        public bool CanClear
        {
            get { return !string.IsNullOrWhiteSpace(SearchText); }
        }

        public StaffingResourceListItem CurrentStaffingResource
        {
            get { return _currentStaffingResource; }
            set
            {
                _currentStaffingResource = value;
                NotifyOfPropertyChange(() => CurrentStaffingResource);
            }
        }

        private IResourceMgtUnitOfWork UnitOfWork
        {
            // Return the shared UoW associated with Guid.Empty
            get { return _unitOfWork ?? (_unitOfWork = _unitOfWorkManager.Get(Guid.Empty)); }
        }

        #region IHandle<SavedMessage> Members

        public void Handle(SavedMessage message)
        {
            // Exit if no StaffingResource was saved
            if (!message.Entities.OfType<DomainModel.StaffingResource>().Any()) return;

            // If there are detached entities than they got deleted.
            var wasDeleted = message.Entities
                .OfType<DomainModel.StaffingResource>()
                .Any(r => r.EntityFacts.EntityState.IsDetached());

            if (wasDeleted)
            {
                Search();
                return;
            }

            if (CurrentStaffingResource != null)
            {
                Search(CurrentStaffingResource.Id);
                return;
            }

            var newStaffResource = message.Entities
                .OfType<DomainModel.StaffingResource>()
                .FirstOrDefault();
            if (newStaffResource != null)
                Search(newStaffResource.Id);
        }

        #endregion

        #region IHarnessAware Members

        public void Setup()
        {
            Start();
        }

        #endregion

        public StaffingResourceSearchViewModel Start()
        {
            Search();
            return this;
        }

        public void Search()
        {
            Search(Guid.Empty);
        }

        public void Search(Guid selection)
        {
            Busy.AddWatch();

            UnitOfWork.Search.Simple(SearchText)
                .ContinueWith(op =>
                                  {
                                      if (op.CompletedSuccessfully)
                                      {
                                          Items = new BindableCollection<StaffingResourceListItem>(op.Result);
                                          CurrentStaffingResource = Items.FirstOrDefault(r => r.Id == selection) ??
                                                                    Items.FirstOrDefault();
                                      }

                                      if (op.HasError)
                                          _errorHandler.HandleError(op.Error);

                                      Busy.RemoveWatch();
                                  });
        }

        public void Clear()
        {
            SearchText = "";
            Search();
        }

        public void SearchTextKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                _unitOfWork = null;
        }
    }
}