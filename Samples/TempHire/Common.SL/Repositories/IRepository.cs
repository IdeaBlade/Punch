using System;
using IdeaBlade.EntityModel;

namespace Common.Repositories
{
    public interface IRepository
    {
        bool HasChanges();

        INotifyCompleted SaveAsync(Action onSuccess = null, Action<Exception> onFail = null);

        void RejectChanges();
    }
}