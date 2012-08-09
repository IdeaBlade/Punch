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

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
using IdeaBlade.Core;
using IdeaBlade.Core.DomainServices;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    [RequiresAuthentication]
    public abstract class EntityBase
    {
        private EntityFacts _entityFacts;

        [NotMapped]
        public EntityFacts EntityFacts
        {
            get { return _entityFacts ?? (_entityFacts = new EntityFacts(this)); }
        }

        [DataMember]
        [ConcurrencyCheck]
        [ConcurrencyStrategy(ConcurrencyStrategy.AutoIncrement)]
        public int RowVersion { get; internal set; }

        public virtual void Validate(VerifierResultCollection validationErrors)
        {
        }

        [BeforeSet]
        internal void RemoveWhiteSpace(IEntityPropertySetInterceptorArgs args)
        {
            if (args.EntityProperty.DataType != typeof(string) || args.Value ==  null)
                return;

            args.Value = ((string) args.Value).Trim();
        }
    }

    [DataContract(IsReference = true)]
    [ClientCanSave(true)]
    public abstract class AuditEntityBase : EntityBase
    {
        [DataMember]
        [Required]
        public DateTime Created { get; set; }

        [DataMember]
        [Required]
        public string CreatedUser { get; set; }

        [DataMember]
        [Required]
        public DateTime Modified { get; set; }

        [DataMember]
        [Required]
        public string ModifyUser { get; set; }
    }

    public class EntityFacts
    {
        private readonly EntityAspect _entityAspect;

        public EntityFacts(object entity)
        {
            _entityAspect = EntityAspect.Wrap(entity);
        }

        public EntityState EntityState
        {
            get { return _entityAspect.EntityState; }
        }

        public bool IsNullEntity
        {
            get { return _entityAspect.IsNullEntity; }
        }

        public bool IsPendingEntity
        {
            get { return _entityAspect.IsPendingEntity; }
        }

        public bool IsNullOrPendingEntity
        {
            get { return _entityAspect.IsNullOrPendingEntity; }
        }

        protected internal EntityAspect EntityAspect
        {
            get { return _entityAspect; }
        }

        public event PropertyChangedEventHandler EntityPropertyChanged
        {
            add { _entityAspect.EntityPropertyChanged += value; }
            remove { _entityAspect.EntityPropertyChanged -= value; }
        }
    }
}