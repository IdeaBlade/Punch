using System;
using Cocktail;
using Action = System.Action;

namespace Common.Repositories
{
    public interface ILookupRepository
    {
        OperationResult InitializeAsync(Action onSuccess = null, Action<Exception> onFail = null);
    }
}