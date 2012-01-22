using System.Collections.Generic;
using System.Linq;
using Cocktail;
using Common.Messages;
using DomainModel;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace TempHire
{
    public class EntityManagerDelegate : EntityManagerDelegate<TempHireEntities>
    {
        public override void OnEntityChanged(TempHireEntities source, EntityChangedEventArgs args)
        {
            EventFns.Publish(new EntityChangedMessage(args.Entity));
        }

        public override void OnSaving(TempHireEntities source, EntitySavingEventArgs args)
        {
            // Add necessary aggregate root object to the save list for validation and concurrency check
            var rootEas = args.Entities.OfType<IHasRoot>()
                .Select(e => EntityAspect.Wrap(e.Root))
                .Distinct()
                .Where(ea => ea != null && !ea.IsChanged && !ea.IsNullOrPendingEntity)
                .ToList();

            rootEas.ForEach(ea => ea.SetModified());
            rootEas.ForEach(ea => args.Entities.Add(ea.Entity));
        }

        public override void OnSaved(TempHireEntities source, EntitySavedEventArgs args)
        {
            if (args.CompletedSuccessfully)
                EventFns.Publish(new SavedMessage(args.Entities));
        }

        public override void Validate(object entity, VerifierResultCollection validationErrors)
        {
            var entityBase = entity as EntityBase;
            if (entityBase != null)
                entityBase.Validate(validationErrors);
        }
    }
}