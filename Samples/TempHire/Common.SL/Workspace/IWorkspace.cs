using Caliburn.Micro;
using IdeaBlade.Core.Composition;

namespace Common.Workspace
{
    [InterfaceExport(typeof (IWorkspace))]
    public interface IWorkspace : IHaveDisplayName
    {
        bool IsDefault { get; }

        int Sequence { get; }
    }
}