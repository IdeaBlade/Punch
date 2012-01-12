using System;
using Common.Dialog;

namespace Common.Errors
{
    public class ErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public void HandleError(Exception ex)
        {
            new ShowMessageResult(ex.Message).Show();
        }

        #endregion
    }
}