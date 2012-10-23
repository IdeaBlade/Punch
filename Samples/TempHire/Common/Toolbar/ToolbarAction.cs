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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Caliburn.Micro;
using Action = System.Action;

namespace Common.Toolbar
{
    public class ToolbarAction : PropertyChangedBase
    {
        private readonly Action _action;
        private readonly object _owner;
        private PropertyInfo _actionEnabledProperty;
        private string _label;

        public ToolbarAction(object owner, string label, Action action)
        {
            if (owner == null) throw new ArgumentNullException("owner");
            if (action == null) throw new ArgumentNullException("action");

            _owner = owner;
            _action = action;
            Label = label;

            EnsureOwner();
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyOfPropertyChange(() => Label);
            }
        }

        public bool CanExecute
        {
            get { return _actionEnabledProperty == null || (bool) _actionEnabledProperty.GetValue(_owner, null); }
        }

        private void EnsureOwner()
        {
            if (!(_owner is INotifyPropertyChanged)) return;

            var actionMethodInfo = _action.Method;
            _actionEnabledProperty = _owner.GetType().GetProperty("Can" + actionMethodInfo.Name, typeof (bool));

            if (_actionEnabledProperty != null)
                (_owner as INotifyPropertyChanged).PropertyChanged += OwnerPropertyChanged;
        }

        private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _actionEnabledProperty.Name)
                NotifyOfPropertyChange(() => CanExecute);
        }

        public void Execute()
        {
            _action();
        }
    }

    public class ToolbarGroup : ObservableCollection<ToolbarAction>
    {
        public ToolbarGroup(int level)
        {
            Level = level;
        }

        public int Level { get; private set; }
    }
}