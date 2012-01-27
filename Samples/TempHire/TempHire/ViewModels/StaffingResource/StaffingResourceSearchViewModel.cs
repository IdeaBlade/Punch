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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Cocktail;
using Common.BusyWatcher;
using Common.Errors;
using Common.Repositories;
using DomainModel.Projections;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceSearchViewModel : Screen, IDiscoverableViewModel, IHarnessAware
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IStaffingResourceRepository _repository;
        private StaffingResourceListItem _currentStaffingResource;
        private BindableCollection<StaffingResourceListItem> _items;

        private string _searchText;

        [ImportingConstructor]
        public StaffingResourceSearchViewModel(IStaffingResourceRepository repository, IErrorHandler errorHandler,
                                       [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IBusyWatcher busy)
        {
            _repository = repository;
            _errorHandler = errorHandler;
            Busy = busy;
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

            _repository.FindStaffingResourcesAsync(SearchText, null,
                                           result =>
                                               {
                                                   Items = new BindableCollection<StaffingResourceListItem>(result);
                                                   CurrentStaffingResource = Items.FirstOrDefault(r => r.Id == selection) ??
                                                                     Items.FirstOrDefault();
                                               },
                                           _errorHandler.HandleError)
                .OnComplete(args => Busy.RemoveWatch());
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
    }
}