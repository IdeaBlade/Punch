using Cocktail;
using DomainModel;

namespace Common.EntityManagerProviders
{
    public class TempHireEntityManagerProvider : BaseEntityManagerProvider<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities();
        }
    }
}