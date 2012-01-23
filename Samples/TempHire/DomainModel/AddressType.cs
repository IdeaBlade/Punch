using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class AddressType : EntityBase
    {
        internal AddressType()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the DisplayName. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Default. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_Default")]
        public bool Default { get; set; }
    }
}