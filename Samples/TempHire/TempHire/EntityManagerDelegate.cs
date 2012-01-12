using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
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
        private IEnumerable<object> _retainedRoots;

        public override void OnEntityChanged(TempHireEntities source, EntityChangedEventArgs args)
        {
            EventFns.Publish(new EntityChangedMessage(args.Entity));
        }

        public override void OnSaving(TempHireEntities source, EntitySavingEventArgs args)
        {
            // Add necessary aggregate root object to the save list for validation and concurrency check
            _retainedRoots = args.Entities.OfType<IHasRoot>()
                .Where(e => e.Root != null && !EntityAspect.Wrap(e.Root).IsChanged)
                .Select(e => e.Root)
                .Distinct()
                .ToList();

            _retainedRoots.ForEach(root => EntityAspect.Wrap(root).SetModified());
            _retainedRoots.ForEach(args.Entities.Add);
        }

        public override void OnSaved(TempHireEntities source, EntitySavedEventArgs args)
        {
            if (args.CompletedSuccessfully)
                EventFns.Publish(new SavedMessage(args.Entities));

            if (args.HasError)
                _retainedRoots.ForEach(root => EntityAspect.Wrap(root).RejectChanges());
        }

        public override void Validate(object entity, VerifierResultCollection validationErrors)
        {
            var entityBase = entity as EntityBase;
            if (entityBase != null)
                entityBase.Validate(validationErrors);
        }
    }
}