using System;
using Cocktail;
using Common.Repositories;
using DomainModel;

namespace TempHire.DesignTimeSupport
{
    public class DesignTimeResourceRepositoryManager : IRepositoryManager<IStaffingResourceRepository>
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        public DesignTimeResourceRepositoryManager(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        public IStaffingResourceRepository Create()
        {
            throw new NotImplementedException();
        }

        public IStaffingResourceRepository GetRepository(Guid key)
        {
            return new StaffingResourceRepository(_entityManagerProvider);
        }

        public void Add(Guid key, IStaffingResourceRepository repository)
        {
            throw new NotImplementedException();
        }
    }
}