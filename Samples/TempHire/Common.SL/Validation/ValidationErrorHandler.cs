using System.ComponentModel.Composition;
using Caliburn.Micro;
using IdeaBlade.Application.Framework.Core.Verification;
using IdeaBlade.Validation;

namespace Common.Validation
{
    public class ValidationErrorHandler : IVerifierResultNotificationService
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ValidationErrorHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnVerificationError(VerifierResultCollection validationErrors)
        {
            if (validationErrors.HasErrors)
                _eventAggregator.Publish(new ValidationErrorMessage(validationErrors));
        }
    }
}