using System.Composition;
using Cocktail;
using IdeaBlade.EntityModel;

namespace NavSample
{
    public interface ICustomerUnitOfWork : IUnitOfWork<Customer>
    {
        bool IsRestored { get; }

        void Restore(EntityCacheState cacheState);

        EntityCacheState GetCacheState();
    }

    [Export(typeof(ICustomerUnitOfWork))]
    [Shared]
    public class CustomerUnitOfWork : UnitOfWork<Customer>, ICustomerUnitOfWork
    {
        [ImportingConstructor]
        public CustomerUnitOfWork(IEntityManagerProvider<NorthwindIBEntities> entityManagerProvider)
            : base(entityManagerProvider)
        {
        }

        public bool IsRestored { get; private set; }

        public void Restore(EntityCacheState cacheState)
        {
            EntityManager.CacheStateManager.RestoreCacheState(cacheState);
            IsRestored = true;
        }

        public EntityCacheState GetCacheState()
        {
            IsRestored = true;
            return EntityManager.CacheStateManager.GetCacheState();
        }
    }
}