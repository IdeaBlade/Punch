using System.ComponentModel.Composition;
using Cocktail;
using Common.Messages;

namespace Common.Validation
{
    public class ValidationErrorMessageProcessor : MessageProcessor<ValidationErrorMessage>
    {
        private readonly ExportFactory<ValidationErrorsViewModel> _viewModelFactory;
        private readonly IDialogManager _dialogManager;

        [ImportingConstructor]
        public ValidationErrorMessageProcessor(ExportFactory<ValidationErrorsViewModel> viewModelFactory, IDialogManager dialogManager)
        {
            _viewModelFactory = viewModelFactory;
            _dialogManager = dialogManager;
        }

        public override void Handle(ValidationErrorMessage message)
        {
            ValidationErrorsViewModel content = _viewModelFactory.CreateExport().Value.Start(message.VerifierResults);
            _dialogManager.ShowDialog(content, DialogButtons.Ok);
        }
    }
}