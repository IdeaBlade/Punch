using System;
using Caliburn.Micro;

namespace TempHire.ViewModels.Resource
{
    public interface IResourceDetailSection : IScreen
    {
        int Index { get; }

        void Start(Guid resourceId);
    }
}