using System;
using IdeaBlade.Core.Composition;

namespace Common.Errors
{
    [InterfaceExport(typeof(IErrorHandler))]
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}