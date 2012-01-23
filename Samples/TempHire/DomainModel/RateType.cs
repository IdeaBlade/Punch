using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class RateType : EntityBase
    {
        internal RateType()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the DisplayName. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Sequence. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_Sequence")]
        public short Sequence { get; set; }
    }
}