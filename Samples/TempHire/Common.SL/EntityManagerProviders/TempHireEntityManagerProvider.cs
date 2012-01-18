using Cocktail;
using DomainModel;

namespace Common.EntityManagerProviders
{
    public class TempHireEntityManagerProvider : EntityManagerProviderBase<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities();
        }
    }
}