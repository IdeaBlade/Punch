using System;

namespace Common.Repositories
{
    public interface IRepositoryManager<T>
    {
        T Create();
        T GetRepository(Guid key);
        void Add(Guid key, T repository);
    }
}