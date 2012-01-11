using Cocktail;
using Security;
using Security.Composition;

namespace Common.EntityManagerProviders
{
    public class DevSecurityEntityManagerProvider : FakeStoreEntityManagerProviderBase<SecurityEntities>
    {
        protected override SecurityEntities CreateEntityManager()
        {
            return new SecurityEntities(compositionContextName: CompositionContextResolver.TempHireFake.Name);
        }
    }
}