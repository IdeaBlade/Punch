using Cocktail;
using DomainModel;
using Security.Composition;

namespace Common.EntityManagerProviders
{
    public class DevTempHireEntityManagerProvider : BaseFakeStoreEntityManagerProvider<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities(compositionContextName: CompositionContextResolver.TempHireFake.Name);
        }
    }
}