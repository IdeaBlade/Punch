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

        public bool IsModified
        {
            get { return _entityAspect.EntityState.IsModified(); }
        }

        public bool IsAdded
        {
            get { return _entityAspect.EntityState.IsAdded(); }
        }

        public bool IsDeleted
        {
            get { return _entityAspect.EntityState.IsDeleted(); }
        }

        public void RejectChanges()
        {
            _entityAspect.RejectChanges();
        }

        public event PropertyChangedEventHandler EntityPropertyChanged
        {
            add { _entityAspect.EntityPropertyChanged += value; }
            remove { _entityAspect.EntityPropertyChanged -= value; }
        }
    }
}