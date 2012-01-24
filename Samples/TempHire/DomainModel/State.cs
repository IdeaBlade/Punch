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
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the ShortName. </summary>
        [DataMember]
        [Required]
        [StringLength(2)]
        public string ShortName { get; set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        public string Name { get; set; }
    }
}