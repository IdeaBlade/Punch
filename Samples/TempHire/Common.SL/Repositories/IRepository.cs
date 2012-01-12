using System;
using Cocktail;
using Action = System.Action;

namespace Common.Repositories
{
    public interface IRepository
    {
        bool HasChanges();

        AsyncOperation SaveAsync(Action onSuccess = null, Action<Exception> onFail = null);

        void RejectChanges();
    }
}