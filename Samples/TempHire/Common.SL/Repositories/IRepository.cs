using System;
using Caliburn.Micro;
using Action = System.Action;

namespace Common.Repositories
{
    public interface IRepository
    {
        bool HasChanges();

        IResult SaveAsync(Action onSuccess = null, Action<Exception> onFail = null);

        void RejectChanges();
    }
}