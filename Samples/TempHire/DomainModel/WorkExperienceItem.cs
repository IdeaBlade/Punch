using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class WorkExperienceItem : EntityBase, IHasRoot
    {
        internal WorkExperienceItem()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the From. </summary>
        [DataMember]
        [Required]
        public DateTime From { get; set; }

        /// <summary>Gets or sets the To. </summary>
        [DataMember]
        [Required]
        public DateTime To { get; set; }

        /// <summary>Gets or sets the PositionTitle. </summary>
        [DataMember]
        [Required]
        public string PositionTitle { get; set; }

        /// <summary>Gets or sets the Company. </summary>
        [DataMember]
        [Required]
        public string Company { get; set; }

        /// <summary>Gets or sets the Location. </summary>
        [DataMember]
        [Required]
        public string Location { get; set; }

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

        internal static WorkExperienceItem Create()
        {
            return new WorkExperienceItem { Id = CombGuid.NewGuid() };
        }
    }
}