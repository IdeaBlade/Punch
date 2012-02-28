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
    public abstract class EntityBase
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

    /// <summary>
    /// Encapsulate access to facts from the DevForce <see cref="EntityAspect"/>.
    /// </summary>
    public class EntityFacts : INotifyPropertyChanged
    {
        private readonly EntityAspect _entityAspect;

        public EntityFacts(object entity)
        {
            _entityAspect = EntityAspect.Wrap(entity);
            _entityAspect.EntityPropertyChanged +=
                (s, args) => RaiseEntityFactsPropertyChanged(string.Empty);
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

        public bool HasErrors
        {
            get { return _entityAspect.ValidationErrors.HasErrors; }
        }

        public EntityAspect.VerifierErrorsCollection ValidationErrors
        {
            get { return _entityAspect.ValidationErrors; }
        }

        protected internal EntityAspect EntityAspect
        {
            get { return _entityAspect; }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            _entityAspect.ForcePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler EntityPropertyChanged
        {
            add { _entityAspect.EntityPropertyChanged += value; }
            remove { _entityAspect.EntityPropertyChanged -= value; }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { EntityFactsPropertyChanged += value; }
            remove { EntityFactsPropertyChanged -= value; }
        }
        protected event PropertyChangedEventHandler EntityFactsPropertyChanged;

        protected void RaiseEntityFactsPropertyChanged(string propertyName)
        {
            if (null == EntityFactsPropertyChanged) return;
            EntityFactsPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}