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

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceSearchViewModel : Screen, IDiscoverableViewModel, IHarnessAware
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IResourceRepository _repository;
        private ResourceListItem _currentResource;
        private BindableCollection<ResourceListItem> _items;

        private string _searchText;

        [ImportingConstructor]
        public ResourceSearchViewModel(IResourceRepository repository, IErrorHandler errorHandler,
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

        public BindableCollection<ResourceListItem> Items
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

        public ResourceListItem CurrentResource
        {
            get { return _currentResource; }
            set
            {
                _currentResource = value;
                NotifyOfPropertyChange(() => CurrentResource);
            }
        }

        #region IHarnessAware Members

        public void Setup()
        {
            Start();
        }

        #endregion

        public ResourceSearchViewModel Start()
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

            _repository.FindResourcesAsync(SearchText, null,
                                           result =>
                                               {
                                                   Items = new BindableCollection<ResourceListItem>(result);
                                                   CurrentResource = Items.FirstOrDefault(r => r.Id == selection) ??
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