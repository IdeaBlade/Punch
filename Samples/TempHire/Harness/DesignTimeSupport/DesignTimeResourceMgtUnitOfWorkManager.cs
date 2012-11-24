using System;
using Cocktail;
using DomainModel;
using DomainServices;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeResourceMgtUnitOfWorkManager : IUnitOfWorkManager<IResourceMgtUnitOfWork>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeResourceMgtUnitOfWorkManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        public IResourceMgtUnitOfWork Create()
        {
            throw new NotImplementedException();
        }

        public IResourceMgtUnitOfWork Get(Guid key)
        {
            return new ResourceMgtUnitOfWork(_entityManagerProvider);
        }

        public void Add(Guid key, IResourceMgtUnitOfWork unitOfWork)
        {
            throw new NotImplementedException();
        }
    }
}