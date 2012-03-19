using System;
using Cocktail;
using DomainModel;
using DomainServices;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeStaffingResourceUnitOfWorkManager : IUnitOfWorkManager<IStaffingResourceUnitOfWork>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeStaffingResourceUnitOfWorkManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        public IStaffingResourceUnitOfWork Create()
        {
            throw new NotImplementedException();
        }

        public IStaffingResourceUnitOfWork Get(Guid key)
        {
            return new StaffingResourceUnitOfWork(_entityManagerProvider);
        }

        public void Add(Guid key, IStaffingResourceUnitOfWork repository)
        {
            throw new NotImplementedException();
        }
    }
}