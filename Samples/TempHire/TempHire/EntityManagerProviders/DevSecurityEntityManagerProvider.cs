using Cocktail;
using Security;
using Security.Composition;

namespace TempHire.EntityManagerProviders
{
    public class DevSecurityEntityManagerProvider : BaseFakeStoreEntityManagerProvider<SecurityEntities>
    {
        protected override SecurityEntities CreateEntityManager()
        {
            return new SecurityEntities(compositionContextName: CompositionContextResolver.TempHireFake.Name);
        }
    }
}