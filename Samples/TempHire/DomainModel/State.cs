using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class State : EntityBase
    {
        internal State()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "State_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the ShortName. </summary>
        [DataMember]
        [Required]
        [StringLengthVerifier(MaxValue = 2, IsRequired = true, ErrorMessageResourceName = "State_ShortName")]
        public string ShortName { get; set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "State_Name")]
        public string Name { get; set; }
    }
}