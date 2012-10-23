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

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.Core;
using TodoServer;
using Windows.UI.Xaml.Controls;

namespace Todo.ViewModels
{
    public class MainPageViewModel : Screen
    {
        private readonly List<TodoItem> _selectedItems = new List<TodoItem>();
        private readonly IUnitOfWork<TodoItem> _unitOfWork;
        private string _description;
        private bool _showArchived;
        private BindableCollection<TodoItem> _todos;

        public MainPageViewModel(IUnitOfWork<TodoItem> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BindableCollection<TodoItem> Todos
        {
            get { return _todos; }
            set
            {
                if (Equals(value, _todos)) return;
                _todos = value;
                NotifyOfPropertyChange(() => Todos);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public bool ShowArchived
        {
            get { return _showArchived; }
            set
            {
                if (value == _showArchived) return;
                _showArchived = value;
                NotifyOfPropertyChange(() => ShowArchived);

                GetTodoItems();
            }
        }

        public bool CanDelete
        {
            get { return _selectedItems.Any(); }
        }

        public bool CanComplete
        {
            get { return _selectedItems.Any() && _selectedItems.All(x => !x.Completed); }
        }

        public bool CanArchive
        {
            get { return Todos != null && Todos.Any(x => x.Completed && !x.Archived); }
        }

        public MainPageViewModel Start()
        {
            UpdateCommands();
            GetTodoItems();
            return this;
        }

        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(Description))
                return;

            var description = Description;
            Description = "";

            var todoItem = await _unitOfWork.Factory.CreateAsync();
            todoItem.Description = description;
            await _unitOfWork.CommitAsync();

            GetTodoItems();
        }

        public async void Delete()
        {
            _unitOfWork.Entities.Delete(_selectedItems);
            await _unitOfWork.CommitAsync();

            GetTodoItems();
        }

        public async void Archive()
        {
            Todos.Where(x => x.Completed).ForEach(x => x.Archived = true);
            await _unitOfWork.CommitAsync();

            GetTodoItems();
        }

        public async void Complete()
        {
            _selectedItems.ForEach(x => x.Completed = true);
            await _unitOfWork.CommitAsync();

            GetTodoItems();
        }

        public void SelectionChanged(SelectionChangedEventArgs args)
        {
            args.AddedItems.ForEach(item => _selectedItems.Add((TodoItem) item));
            args.RemovedItems.ForEach(item => _selectedItems.Remove((TodoItem) item));

            UpdateCommands();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Start();
        }

        private async void GetTodoItems()
        {
            _selectedItems.Clear();

            IEnumerable<TodoItem> todos;
            if (_showArchived)
                todos = await _unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.Description));
            else
                todos = await _unitOfWork.Entities.FindAsync(x => !x.Archived, q => q.OrderBy(x => x.Description));

            Todos = new BindableCollection<TodoItem>(todos);

            UpdateCommands();
        }

        private void UpdateCommands()
        {
            NotifyOfPropertyChange(() => CanComplete);
            NotifyOfPropertyChange(() => CanDelete);
            NotifyOfPropertyChange(() => CanArchive);
        }
    }
}