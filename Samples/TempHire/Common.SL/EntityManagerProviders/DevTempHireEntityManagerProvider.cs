using Cocktail;
using DomainModel;
using Security.Composition;

namespace Common.EntityManagerProviders
{
    public class DevTempHireEntityManagerProvider : FakeStoreEntityManagerProviderBase<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities(compositionContextName: CompositionContextResolver.TempHireFake.Name);
        }
    }
}