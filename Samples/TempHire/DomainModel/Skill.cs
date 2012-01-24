using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class Skill : EntityBase, IHasRoot
    {
        internal Skill()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Description. </summary>
        [DataMember]
        [Required]
        public string Description { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [Required]
        public Guid ResourceId { get; set; }

        /// <summary>Gets or sets the Resource. </summary>
        [DataMember]
        public Resource Resource { get; set; }

        #region IHasRoot Members

        public object Root
        {
            get { return Resource; }
        }

        #endregion

        internal static Skill Create()
        {
            return new Skill { Id = CombGuid.NewGuid() };
        }
    }
}