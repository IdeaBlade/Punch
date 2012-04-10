using System;
using Cocktail;
using DomainModel;
using DomainServices;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeStaffingResourceUnitOfWorkManager : IDomainUnitOfWorkManager<IDomainUnitOfWork>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeStaffingResourceUnitOfWorkManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        public IDomainUnitOfWork Create()
        {
            throw new NotImplementedException();
        }

        public IDomainUnitOfWork Get(Guid key)
        {
            return new DomainUnitOfWork(_entityManagerProvider);
        }

        public void Add(Guid key, IDomainUnitOfWork repository)
        {
            throw new NotImplementedException();
        }
    }
}