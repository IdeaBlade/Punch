using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
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
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the DisplayName. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Default. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "AddressType_Default")]
        public bool Default { get; set; }

        /// <summary>Gets the Address. </summary>
        [DataMember]
        [InverseProperty("AddressType")]
        public RelatedEntityList<Address> Address { get; set; }
    }
}