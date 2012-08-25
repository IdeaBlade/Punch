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
using TodoServer;

namespace Todo.ViewModels
{
    public class MainPageViewModel : Screen
    {
        private readonly IUnitOfWork<TodoItem> _unitOfWork;
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

        public MainPageViewModel Start()
        {
            GetTodoItems();
            return this;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Start();
        }

        private async void GetTodoItems(bool showArchived = false)
        {
            IEnumerable<TodoItem> todos;
            if (showArchived)
                todos = await _unitOfWork.Entities.AllAsync(q => q.OrderBy(x => x.Description));
            else
                todos = await _unitOfWork.Entities.FindAsync(x => !x.Archived, q => q.OrderBy(x => x.Description));

            Todos = new BindableCollection<TodoItem>(todos);
        }
    }
}