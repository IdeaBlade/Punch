using System;
using System.ComponentModel.Composition;
using Cocktail;

namespace Common.Errors
{
    [Export(typeof(IErrorHandler))]
    public class ErrorHandler : IErrorHandler
    {
        private readonly IDialogManager _dialogManager;

        [ImportingConstructor]
        public ErrorHandler(IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        #region IErrorHandler Members

        public void HandleError(Exception ex)
        {
            _dialogManager.ShowMessage(ex.Message);
        }

        #endregion
    }
}