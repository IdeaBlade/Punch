using System.Collections.Generic;

namespace Common.Toolbar
{
    public interface IToolbarManager
    {
        IEnumerable<ToolbarAction> Actions { get; }

        IEnumerable<ToolbarGroup> Groups { get; }

        void AddGroup(ToolbarGroup group);

        void RemoveGroup(ToolbarGroup group);
    }
}