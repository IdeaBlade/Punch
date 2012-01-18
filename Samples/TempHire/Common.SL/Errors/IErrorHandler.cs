using System;
using IdeaBlade.Core.Composition;

namespace Common.Errors
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}