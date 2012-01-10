using DomainModel;
using IdeaBlade.Application.Framework.Core.Persistence;
using Security.Composition;

namespace TempHire.EntityManagerProviders
{
    public class DevTempHireEntityManagerProvider : BaseFakeStoreEntityManagerProvider<TempHireEntities>
    {
        protected override TempHireEntities CreateEntityManager()
        {
            return new TempHireEntities(compositionContextName: CompositionContextResolver.TempHireFake.Name);
        }
    }
}