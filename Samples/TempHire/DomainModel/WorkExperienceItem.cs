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
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the From. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_From")]
        public DateTime From { get; set; }

        /// <summary>Gets or sets the To. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_To")]
        public DateTime To { get; set; }

        /// <summary>Gets or sets the PositionTitle. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_PositionTitle")]
        public string PositionTitle { get; set; }

        /// <summary>Gets or sets the Company. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_Company")]
        public string Company { get; set; }

        /// <summary>Gets or sets the Location. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_Location")]
        public string Location { get; set; }

        /// <summary>Gets or sets the Description. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "WorkExperienceItem_ResourceId")]
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