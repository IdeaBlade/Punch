using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    public class RateType
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
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the DisplayName. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the Sequence. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "RateType_Sequence")]
        public short Sequence { get; set; }

        /// <summary>Gets the Rates. </summary>
        [DataMember]
        [InverseProperty("RateType")]
        public RelatedEntityList<Rate> Rates { get; set; }
    }
}