using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using Common.Repositories;

namespace TempHire.ViewModels.Resource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceSummaryViewModel : ResourceScreenBase
    {
        private readonly ExportFactory<ResourceNameEditorViewModel> _nameEditorFactory;
        private readonly IDialogManager _dialogManager;

        [ImportingConstructor]
        public ResourceSummaryViewModel(IRepositoryManager<IResourceRepository> repositoryManager,
                                        ExportFactory<ResourceNameEditorViewModel> nameEditorFactory,
                                        IErrorHandler errorHandler, IDialogManager dialogManager)
            : base(repositoryManager, errorHandler)
        {
            _nameEditorFactory = nameEditorFactory;
            _dialogManager = dialogManager;
        }

        public IEnumerable<IResult> EditName()
        {
            ResourceNameEditorViewModel nameEditor = _nameEditorFactory.CreateExport().Value;
            yield return _dialogManager.ShowDialog(nameEditor.Start(Resource.Id), DialogButtons.OkCancel);

            Resource.FirstName = nameEditor.FirstName;
            Resource.MiddleName = nameEditor.MiddleName;
            Resource.LastName = nameEditor.LastName;
        }
    }
}