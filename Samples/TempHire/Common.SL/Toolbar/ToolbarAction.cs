using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Caliburn.Micro;
using Caliburn.Micro.Extensions;
using Action = System.Action;

namespace Common.Toolbar
{
    public class ToolbarAction : PropertyChangedBase
    {
        private readonly Action _action;
        private readonly Func<IEnumerable<IResult>> _actionCoroutine;
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

        public ToolbarAction(object owner, string label, Func<IEnumerable<IResult>> action)
        {
            if (owner == null) throw new ArgumentNullException("owner");
            if (action == null) throw new ArgumentNullException("action");

            _owner = owner;
            _actionCoroutine = action;
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

            MethodInfo actionMethodInfo = _actionCoroutine != null ? _actionCoroutine.Method : _action.Method;
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
            if (_actionCoroutine != null)
                _actionCoroutine().ToSequential().Execute(null);
            else
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