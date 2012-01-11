using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using IdeaBlade.Validation;

namespace Common.Validation
{
    public class ValidationErrorHandler : IValidationErrorNotification
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ValidationErrorHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnValidationError(VerifierResultCollection validationErrors)
        {
            if (validationErrors.HasErrors)
                _eventAggregator.Publish(new ValidationErrorMessage(validationErrors));
        }
    }
}