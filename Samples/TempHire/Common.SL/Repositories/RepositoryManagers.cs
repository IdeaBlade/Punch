using System;
using System.ComponentModel.Composition;
using Cocktail;

namespace Common.Repositories
{
    public class RepositoryManager<T> : ObjectManager<Guid,T>, IRepositoryManager<T>
    {
        public T GetRepository(Guid key)
        {
            return GetObject(key);
        }
    }

    /// <summary>
    /// Used to share instances of the ResourceRepository among composed view models.
    /// </summary>
    [Export(typeof(IRepositoryManager<IResourceRepository>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ResourceRepositoryManager : RepositoryManager<IResourceRepository>
    {
    }
}