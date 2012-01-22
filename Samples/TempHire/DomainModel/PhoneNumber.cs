using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class PhoneNumber : EntityBase, IHasRoot
    {
        internal PhoneNumber()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumber_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the AreaCode. </summary>
        [DataMember]
        [StringLengthVerifier(MaxValue = 3, MinValue = 3, IsRequired = true, ErrorMessageResourceName = "PhoneNumber_AreaCode")]
        public string AreaCode { get; set; }

        /// <summary>Gets or sets the Number. </summary>
        [DataMember]
        [StringLengthVerifier(MaxValue = 7, MinValue = 7, IsRequired = true, ErrorMessageResourceName = "PhoneNumber_Number")]
        public string Number { get; set; }

        /// <summary>Gets or sets the PhoneNumberTypeId. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumber_PhoneNumberTypeId")]
        public Guid PhoneNumberTypeId { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumber_ResourceId")]
        public Guid ResourceId { get; set; }

        /// <summary>Gets or sets the Primary. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "PhoneNumber_Primary")]
        public bool Primary { get; set; }

        /// <summary>Gets or sets the Resource. </summary>
        [DataMember]
        public Resource Resource { get; set; }

        /// <summary>Gets or sets the PhoneNumberType. </summary>
        [DataMember]
        public PhoneNumberType PhoneNumberType { get; set; }

        #region IHasRoot Members

        public object Root
        {
            get { return Resource; }
        }

        #endregion

        #region EntityPropertyNames

        public class EntityPropertyNames : Entity.EntityPropertyNames
        {
            public const String Id = "Id";
            public const String AreaCode = "AreaCode";
            public const String Number = "Number";
            public const String PhoneNumberTypeId = "PhoneNumberTypeId";
            public const String ResourceId = "ResourceId";
            public const String Primary = "Primary";
            public const String Resource = "Resource";
            public const String PhoneNumberType = "PhoneNumberType";
            public const String PrimaryResources = "PrimaryResources";
        }

        #endregion EntityPropertyNames

        internal static PhoneNumber Create(PhoneNumberType type)
        {
            return new PhoneNumber { Id = CombGuid.NewGuid(), PhoneNumberTypeId = type.Id };
        }
    }
}