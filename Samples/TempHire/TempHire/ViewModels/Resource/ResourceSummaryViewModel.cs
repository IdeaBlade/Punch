using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Common.Dialog;
using Common.Errors;
using Common.Repositories;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceSummaryViewModel : ResourceScreenBase
    {
        private readonly ExportFactory<ResourceNameEditorViewModel> _nameEditorFactory;

        [ImportingConstructor]
        public ResourceSummaryViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                        ExportFactory<ResourceNameEditorViewModel> nameEditorFactory,
                                        IErrorHandler errorHandler)
            : base(repositoryManager, errorHandler)
        {
            _nameEditorFactory = nameEditorFactory;
        }

        public IEnumerable<IResult> EditName()
        {
            ResourceNameEditorViewModel nameEditor = _nameEditorFactory.CreateExport().Value;
            yield return new ShowDialogResult(nameEditor.Start(Resource.Id));

            Resource.FirstName = nameEditor.FirstName;
            Resource.MiddleName = nameEditor.MiddleName;
            Resource.LastName = nameEditor.LastName;
        }
    }
}