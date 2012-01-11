using Cocktail;
using IdeaBlade.EntityModel;

namespace TempHire
{
    public class SyncInterceptor : EntityManagerSyncInterceptor
    {
        public override bool ShouldExportEntity(object entity)
        {
            return true;
        }

        public override bool ShouldImportEntity(object entity)
        {
            // Only import if the importing EntityManager holds a copy of the entity in its cache
            return EntityManager.FindEntity(EntityAspect.Wrap(entity).EntityKey) != null;
        }
    }
}