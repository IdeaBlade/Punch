using System;
using Caliburn.Micro;
using Action = System.Action;

namespace Common.Repositories
{
    public interface ILookupRepository
    {
        IResult InitializeAsync(Action onSuccess = null, Action<Exception> onFail = null);
    }
}