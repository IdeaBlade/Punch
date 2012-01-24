using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class Rate : EntityBase, IHasRoot
    {
        internal Rate()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Amount. </summary>
        [DataMember]
        [Required]
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the RateTypeId. </summary>
        [DataMember]
        [Required]
        public Guid RateTypeId { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [Required]
        public Guid ResourceId { get; set; }

        /// <summary>Gets or sets the RateType. </summary>
        [DataMember]
        public RateType RateType { get; set; }

        /// <summary>Gets or sets the Resource. </summary>
        [DataMember]
        public Resource Resource { get; set; }

        #region IHasRoot Members

        public object Root
        {
            get { return Resource; }
        }

        #endregion

        internal static Rate Create(RateType type)
        {
            return new Rate { Id = CombGuid.NewGuid(), RateTypeId = type.Id };
        }
    }
}