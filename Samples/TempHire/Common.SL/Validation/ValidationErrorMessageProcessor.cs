using System.ComponentModel.Composition;
using Common.Dialog;
using Common.Messages;

namespace Common.Validation
{
    public class ValidationErrorMessageProcessor : MessageProcessor<ValidationErrorMessage>
    {
        private readonly ExportFactory<ValidationErrorsViewModel> _viewModelFactory;

        [ImportingConstructor]
        public ValidationErrorMessageProcessor(ExportFactory<ValidationErrorsViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public override void Handle(ValidationErrorMessage message)
        {
            ValidationErrorsViewModel content = _viewModelFactory.CreateExport().Value.Start(message.VerifierResults);
            new ShowDialogResult(content, true).Show();
        }
    }
}