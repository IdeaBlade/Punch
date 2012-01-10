using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace Common.Toolbar
{
    [Export(typeof (IToolbarManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ToolbarManager : PropertyChangedBase, IToolbarManager
    {
        private readonly List<ToolbarGroup> _groups;

        public ToolbarManager()
        {
            _groups = new List<ToolbarGroup>();
        }

        #region IToolbarManager Members

        public IEnumerable<ToolbarAction> Actions
        {
            get { return Groups.SelectMany(g => g); }
        }

        public IEnumerable<ToolbarGroup> Groups
        {
            get { return _groups.OrderBy(g => g.Level); }
        }

        public void AddGroup(ToolbarGroup @group)
        {
            _groups.Add(@group);
            NotifyOfPropertyChange(() => Actions);
            NotifyOfPropertyChange(() => Groups);
        }

        public void RemoveGroup(ToolbarGroup @group)
        {
            _groups.Remove(@group);
            NotifyOfPropertyChange(() => Actions);
            NotifyOfPropertyChange(() => Groups);
        }

        #endregion
    }
}