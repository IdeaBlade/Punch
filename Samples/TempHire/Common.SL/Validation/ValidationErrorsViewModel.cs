using System.ComponentModel.Composition;
using Caliburn.Micro;
using IdeaBlade.Validation;

namespace Common.Validation
{
    [Export]
    public class ValidationErrorsViewModel : Screen
    {
        private VerifierResultCollection _verifierResults;

        public VerifierResultCollection VerifierResults
        {
            get { return _verifierResults; }
            private set
            {
                _verifierResults = value;
                NotifyOfPropertyChange(() => VerifierResults);
            }
        }

        public ValidationErrorsViewModel Start(VerifierResultCollection verifierResults)
        {
            VerifierResults = verifierResults;
            return this;
        }
    }
}