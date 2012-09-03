using System.Composition;
using Cocktail;

namespace NavSample
{
    [Export(typeof (IUnitOfWork<Customer>))]
    [Shared]
    public class CustomerUnitOfWork : UnitOfWork<Customer>
    {
        [ImportingConstructor]
        public CustomerUnitOfWork(IEntityManagerProvider<NorthwindIBEntities> entityManagerProvider)
            : base(entityManagerProvider)
        {
        }
    }
}