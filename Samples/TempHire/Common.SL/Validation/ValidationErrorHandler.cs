using Cocktail;
using IdeaBlade.Validation;

namespace Common.Validation
{
    public class ValidationErrorHandler : IValidationErrorNotification
    {
        #region IValidationErrorNotification Members

        public void OnValidationError(VerifierResultCollection validationErrors)
        {
            if (validationErrors.HasErrors)
                EventFns.Publish(new ValidationErrorMessage(validationErrors));
        }

        #endregion
    }
}