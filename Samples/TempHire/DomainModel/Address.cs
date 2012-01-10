using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.Application.Framework.Core.Verification;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    public class Address : IHasRoot, ICustomVerifier
    {
        internal Address()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the Address1. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_Address1")]
        public string Address1 { get; set; }

        /// <summary>Gets or sets the Address2. </summary>
        [DataMember]
        public string Address2 { get; set; }

        /// <summary>Gets or sets the City. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_City")]
        public string City { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [ForeignKey("Resource")]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_ResourceId")]
        public Guid ResourceId { get; set; }

        /// <summary>Gets or sets the AddressTypeId. </summary>
        [DataMember]
        [ForeignKey("AddressType")]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_AddressTypeId")]
        public Guid AddressTypeId { get; set; }

        /// <summary>Gets or sets the Zipcode. </summary>
        [DataMember]
        [StringLengthVerifier(MaxValue = 10, MinValue = 5, IsRequired = true, ErrorMessageResourceName = "Address_Zipcode")]
        public string Zipcode { get; set; }

        /// <summary>Gets or sets the Primary. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Address_Primary")]
        public bool Primary { get; set; }

        /// <summary>Gets or sets the StateId. </summary>
        [DataMember]
        [Required]
        [ForeignKey("State")]
        public Guid StateId { get; set; }

        /// <summary>Gets or sets the Resource. </summary>
        [DataMember]
        [InverseProperty("Addresses")]
        public Resource Resource { get; set; }

        /// <summary>Gets or sets the AddressType. </summary>
        [DataMember]
        [InverseProperty("Address")]
        public AddressType AddressType { get; set; }

        /// <summary>Gets or sets the State. </summary>
        [DataMember]
        [Required]
        public State State { get; set; }

        #region IHasRoot Members

        public object Root
        {
            get { return !EntityAspect.Wrap(Resource).IsNullEntity ? Resource : null; }
        }

        #endregion

        #region EntityPropertyNames

        public class EntityPropertyNames : Entity.EntityPropertyNames
        {
            public const String Id = "Id";
            public const String Address1 = "Address1";
            public const String Address2 = "Address2";
            public const String City = "City";
            public const String ResourceId = "ResourceId";
            public const String AddressTypeId = "AddressTypeId";
            public const String Zipcode = "Zipcode";
            public const String Primary = "Primary";
            public const String StateId = "StateId";
            public const String Resource = "Resource";
            public const String AddressType = "AddressType";
            public const String PrimaryResources = "PrimaryResources";
            public const String State = "State";
        }

        #endregion EntityPropertyNames

        internal static Address Create(AddressType type)
        {
            return new Address { Id = CombGuid.NewGuid(), AddressTypeId = type.Id };
        }

        public void Verify(VerifierResultCollection verifierResultCollection)
        {
            if (EntityAspect.Wrap(State).IsNullOrPendingEntity)
                verifierResultCollection.Add(new VerifierResult(VerifierResultCode.Error, "State is required", "State"));
        }
    }
}