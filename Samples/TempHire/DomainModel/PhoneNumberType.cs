using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class PhoneNumberType : EntityBase
    {
        internal PhoneNumberType()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumberType_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumberType_Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the Default. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumberType_Default")]
        public bool Default { get; set; }
    }
}