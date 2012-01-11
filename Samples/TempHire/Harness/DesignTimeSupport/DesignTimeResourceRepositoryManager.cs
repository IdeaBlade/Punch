using System;
using Cocktail;
using Common.Repositories;
using DomainModel;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeResourceRepositoryManager : IRepositoryManager<IResourceRepository>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeResourceRepositoryManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        public IResourceRepository Create()
        {
            throw new NotImplementedException();
        }

        public IResourceRepository GetRepository(Guid key)
        {
            return new ResourceRepository(_entityManagerProvider);
        }

        public void Add(Guid key, IResourceRepository repository)
        {
            throw new NotImplementedException();
        }
    }
}