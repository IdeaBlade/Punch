using Common.SL.Errors;
using DomainModel.Repositories;
using TempHire.Repositories;
using TempHire.ViewModels.Dialog;

namespace TempHire.ViewModels.Resource
{
    public abstract class ResourceDetailSection : ResourceScreenBase
    {
        protected ResourceDetailSection(int index, IRepositoryManager<IResourceRepository> repositoryManager,
                                        DialogHostViewModel dialogHost, IErrorHandler errorHandler)
            : base(repositoryManager, dialogHost, errorHandler)
        {
            Index = index;
        }

        public int Index { get; private set; }
    }
}