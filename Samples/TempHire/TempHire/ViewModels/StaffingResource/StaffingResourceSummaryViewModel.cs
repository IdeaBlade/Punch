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

using System.ComponentModel.Composition;
using Cocktail;
using DomainServices;

namespace TempHire.ViewModels.StaffingResource
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceSummaryViewModel : StaffingResourceScreenBase
    {
        private readonly IDialogManager _dialogManager;
        private readonly ExportFactory<StaffingResourceNameEditorViewModel> _nameEditorFactory;

        [ImportingConstructor]
        public StaffingResourceSummaryViewModel(IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork> unitOfWorkManager,
                                                ExportFactory<StaffingResourceNameEditorViewModel> nameEditorFactory,
                                                IDialogManager dialogManager)
            : base(unitOfWorkManager)
        {
            _nameEditorFactory = nameEditorFactory;
            _dialogManager = dialogManager;
        }

        public async void EditName()
        {
            var nameEditor = _nameEditorFactory.CreateExport().Value;
            await _dialogManager.ShowDialogAsync(nameEditor.Start(StaffingResource.Id), DialogButtons.OkCancel);

            StaffingResource.FirstName = nameEditor.FirstName;
            StaffingResource.MiddleName = nameEditor.MiddleName;
            StaffingResource.LastName = nameEditor.LastName;
        }
    }
}