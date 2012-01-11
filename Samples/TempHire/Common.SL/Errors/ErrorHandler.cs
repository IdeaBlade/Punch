using System;
using Common.Dialog;

namespace Common.Errors
{
    public class ErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public void HandleError(Exception ex)
        {
            var result = new ShowMessageResult("Unexpected Error", ex.Message);
            result.Execute(null);
        }

        #endregion
    }
}