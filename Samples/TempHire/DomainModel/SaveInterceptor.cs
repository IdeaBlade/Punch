// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Linq;
using Cocktail;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Server;
using IdeaBlade.Validation;

namespace DomainModel
{
    public class SaveInterceptor : EntityServerSaveInterceptor
    {
        // Don't allow saving of any entity type by default. CanSave attribute must be used to allow saving of specific types
        protected override bool DefaultAuthorization
        {
            get { return false; }
        }

        protected override bool ValidateSave()
        {
            base.ValidateSave();

            // Create a sandbox to do the validation in.
            var em = new EntityManager(EntityManager);
            em.CacheStateManager.RestoreCacheState(EntityManager.CacheStateManager.GetCacheState());

            // Find all entities supporting custom validation                
            var entities =
                em.FindEntities(EntityState.AllButDetached).OfType<EntityBase>().ToList();

            foreach (var e in entities)
            {
                var entityAspect = EntityAspect.Wrap(e);
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

        protected override bool ExecuteSave()
        {
            // Update Audit columns
            EntityManager.FindEntities<AuditEntityBase>(EntityState.Added)
                .ForEach(e =>
                             {
                                 e.Created = e.Modified = SystemTime.Now;
                                 e.CreatedUser = e.ModifyUser = Principal.Identity.Name;
                             });

            EntityManager.FindEntities<AuditEntityBase>(EntityState.Modified)
                .ForEach(e =>
                             {
                                 e.Modified = SystemTime.Now;
                                 e.ModifyUser = Principal.Identity.Name;
                             });

            return base.ExecuteSave();
        }
    }
}