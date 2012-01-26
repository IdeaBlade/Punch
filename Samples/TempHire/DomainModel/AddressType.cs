using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

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
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Name. </summary>
        [DataMember]
        [Required]
        public string Name { get; set; }

        /// <summary>Gets or sets the DisplayName. </summary>
        [DataMember]
        [Required]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Default. </summary>
        [DataMember]
        [Required]
        public bool Default { get; set; }
    }
}