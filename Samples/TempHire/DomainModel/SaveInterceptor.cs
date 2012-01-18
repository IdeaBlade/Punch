using System.Collections.Generic;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Server;
using IdeaBlade.Validation;

namespace DomainModel
{
    public class SaveInterceptor : EntityServerSaveInterceptor
    {
        protected override bool ValidateSave()
        {
            base.ValidateSave();

            // Create a sandox to do the validation in.
            var em = new EntityManager(EntityManager);
            em.CacheStateManager.RestoreCacheState(EntityManager.CacheStateManager.GetCacheState());

            // Find all entities supporting custom validation                
            List<EntityBase> entities =
                em.FindEntities(EntityState.AllButDetached).OfType<EntityBase>().ToList();

            foreach (EntityBase e in entities)
            {
                EntityAspect entityAspect = EntityAspect.Wrap(e);
                if (entityAspect.EntityState.IsDeletedOrDetached()) continue;

                var validationErrors = new VerifierResultCollection();
                e.Validate(validationErrors);

                validationErrors =
                    new VerifierResultCollection(entityAspect.ValidationErrors.Concat(validationErrors.Errors));
                validationErrors.Where(vr => !entityAspect.ValidationErrors.Contains(vr))
                    .ForEach(entityAspect.ValidationErrors.Add);

                if (validationErrors.HasErrors)
                    throw new EntityServerException(validationErrors.Select(v => v.Message).ToAggregateString("\n"),
                                                    null,
                                                    PersistenceOperation.Save, PersistenceFailure.Validation);
            }

            return true;
        }
    }
}