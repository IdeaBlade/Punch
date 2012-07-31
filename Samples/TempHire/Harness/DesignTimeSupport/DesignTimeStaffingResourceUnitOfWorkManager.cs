using System;
using Cocktail;
using DomainModel;
using DomainServices;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeStaffingResourceUnitOfWorkManager : IResourceMgtUnitOfWorkManager<IResourceMgtUnitOfWork>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeStaffingResourceUnitOfWorkManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
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

        public void Add(Guid key, IResourceMgtUnitOfWork repository)
        {
            throw new NotImplementedException();
        }
    }
}