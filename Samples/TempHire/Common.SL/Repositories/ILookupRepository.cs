using System;
using IdeaBlade.EntityModel;

namespace Common.Repositories
{
    public interface ILookupRepository
    {
        INotifyCompleted InitializeAsync(Action onSuccess = null, Action<Exception> onFail = null);
    }
}