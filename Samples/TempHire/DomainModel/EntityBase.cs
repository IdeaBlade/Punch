using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    public class EntityBase
    {
        private EntityFacts _entityFacts;

        [NotMapped]
        public EntityFacts EntityFacts
        {
            get { return _entityFacts ?? (_entityFacts = new EntityFacts(this)); }
        }

        public virtual void Validate(VerifierResultCollection validationErrors)
        {
        }
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