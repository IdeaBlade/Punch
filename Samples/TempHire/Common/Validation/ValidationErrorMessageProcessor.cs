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
using Common.Messages;

namespace Common.Validation
{
    public class ValidationErrorMessageProcessor : MessageProcessor<ValidationErrorMessage>
    {
        private readonly IDialogManager _dialogManager;
        private readonly ExportFactory<ValidationErrorsViewModel> _viewModelFactory;

        [ImportingConstructor]
        public ValidationErrorMessageProcessor(ExportFactory<ValidationErrorsViewModel> viewModelFactory,
                                               IDialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
        }

        public override void Handle(ValidationErrorMessage message)
        {
            var content = _viewModelFactory.CreateExport().Value.Start(message.VerifierResults);
            _dialogManager.ShowDialogAsync(content, DialogButtons.Ok);
        }
    }
}