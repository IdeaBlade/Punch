using IdeaBlade.Validation;

namespace Common.Validation
{
    public class ValidationErrorMessage
    {
        public ValidationErrorMessage(VerifierResultCollection verifierResults)
        {
            VerifierResults = verifierResults;
        }

        public VerifierResultCollection VerifierResults { get; private set; }
    }
}