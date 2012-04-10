//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Cocktail;
using Common.Messages;
using Common.Security;
using DomainModel;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace TempHire
{
    public class EntityManagerDelegate : EntityManagerDelegate<TempHireEntities>
    {
        private readonly IUserService _userService;

        [ImportingConstructor]
        public EntityManagerDelegate(IUserService userService)
        {
            _userService = userService;
        }

        public override void OnEntityChanged(TempHireEntities source, EntityChangedEventArgs args)
        {
            EventFns.Publish(new EntityChangedMessage(args.Entity));
        }

        public override void OnSaving(TempHireEntities source, EntitySavingEventArgs args)
        {
            // Add necessary aggregate root object to the save list for validation and concurrency check
            List<EntityAspect> rootEas = args.Entities.OfType<IHasRoot>()
                .Select(e => EntityAspect.Wrap(e.Root))
                .Distinct()
                .Where(ea => ea != null && !ea.IsChanged && !ea.IsNullOrPendingEntity)
                .ToList();

            rootEas.ForEach(ea => ea.SetModified());
            rootEas.ForEach(ea => args.Entities.Add(ea.Entity));

            // Update Audit columns
            args.Entities.OfType<AuditEntityBase>()
                .Where(e => e.EntityFacts.EntityState.IsAdded())
                .ForEach(e =>
                             {
                                 e.Created = e.Modified = SystemTime.Now;
                                 e.CreatedUser = e.ModifyUser = _userService.CurrentUser.Name;
                             });

            args.Entities.OfType<AuditEntityBase>()
                .Where(e => e.EntityFacts.EntityState.IsModified())
                .ForEach(e =>
                             {
                                 e.Modified = SystemTime.Now;
                                 e.ModifyUser = _userService.CurrentUser.Name;
                             });
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