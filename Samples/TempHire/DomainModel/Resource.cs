using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using IdeaBlade.Aop;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.Application.Framework.Core.Verification;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    public class Resource : ICustomVerifier
    {
        internal Resource()
        {
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(MiddleName)
                           ? string.Format("{0} {1} {2}", FirstName.Trim(), MiddleName.Trim(), LastName.Trim())
                           : string.Format("{0} {1}", FirstName.Trim(), LastName.Trim());
            }
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RequiredValueVerifier(ErrorMessageResourceName = "Resource_Id")]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the FirstName. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Resource_FirstName")]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the MiddleName. </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>Gets or sets the LastName. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Resource_LastName")]
        public string LastName { get; set; }

        /// <summary>Gets or sets the Summary. </summary>
        [DataMember]
        [RequiredValueVerifier(ErrorMessageResourceName = "Resource_Summary")]
        public string Summary { get; set; }

        /// <summary>Gets or sets the Timestamp. </summary>
        [DataMember]
        [ConcurrencyCheck]
        [ConcurrencyStrategy(ConcurrencyStrategy.AutoDateTime)]
        [RequiredValueVerifier(ErrorMessageResourceName = "Resource_Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>Gets the Addresses. </summary>
        [DataMember]
        [InverseProperty("Resource")]
        public RelatedEntityList<Address> Addresses { get; set; }

        /// <summary>Gets the PhoneNumbers. </summary>
        [DataMember]
        [InverseProperty("Resource")]
        public RelatedEntityList<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>Gets the Rates. </summary>
        [DataMember]
        [InverseProperty("Resource")]
        public RelatedEntityList<Rate> Rates { get; set; }

        /// <summary>Gets the WorkExperience. </summary>
        [DataMember]
        [InverseProperty("Resource")]
        public RelatedEntityList<WorkExperienceItem> WorkExperience { get; set; }

        /// <summary>Gets the Skills. </summary>
        [DataMember]
        [InverseProperty("Resource")]
        public RelatedEntityList<Skill> Skills { get; set; }

        /// <summary>Gets or sets the PrimaryAddress. </summary>
        [NotMapped]
        public Address PrimaryAddress
        {
            get { return Addresses.FirstOrDefault(a => a.Primary); }
            set
            {
                if (value.Resource != this)
                    throw new InvalidOperationException("Address is not associated with this resource.");

                Addresses.Where(a => a.Primary).ForEach(a => a.Primary = false);
                value.Primary = true;
            }
        }

        /// <summary>Gets or sets the PrimaryPhoneNumber. </summary>
        [NotMapped]
        public PhoneNumber PrimaryPhoneNumber
        {
            get { return PhoneNumbers.FirstOrDefault(a => a.Primary); } 
            set
            {
                if (value.Resource != this)
                    throw new InvalidOperationException("PhoneNumber is not associated with this resource.");

                PhoneNumbers.Where(p => p.Primary).ForEach(p => p.Primary = false);
                value.Primary = true;
            }
        }

        #region ICustomVerifier Members

        public void Verify(VerifierResultCollection verifierResultCollection)
        {
            if (Addresses.Count == 0)
                verifierResultCollection.Add(new VerifierResult(false, "Resource must have at least one address",
                                                                "Addresses"));

            if (PhoneNumbers.Count == 0)
                verifierResultCollection.Add(new VerifierResult(false, "Resource must have at least one phone number",
                                                                "PhoneNumbers"));
        }

        #endregion

        public static Resource Create()
        {
            return new Resource {Id = CombGuid.NewGuid()};
        }

        public Address AddAddress(AddressType type)
        {
            Address address = Address.Create(type);
            Addresses.Add(address);

            EnsureModified();
            return address;
        }

        public void DeleteAddress(Address address)
        {
            Addresses.Remove(address);
            EntityAspect.Wrap(address).Delete();

            EnsureModified();
        }

        public PhoneNumber AddPhoneNumber(PhoneNumberType type)
        {
            PhoneNumber phoneNumber = PhoneNumber.Create(type);
            PhoneNumbers.Add(phoneNumber);

            EnsureModified();
            return phoneNumber;
        }

        public void DeletePhoneNumber(PhoneNumber phoneNumber)
        {
            PhoneNumbers.Remove(phoneNumber);
            EntityAspect.Wrap(phoneNumber).Delete();

            EnsureModified();
        }

        public Rate AddRate(RateType type)
        {
            Rate rate = Rate.Create(type);
            Rates.Add(rate);

            EnsureModified();
            return rate;
        }

        public void DeleteRate(Rate rate)
        {
            Rates.Remove(rate);
            EntityAspect.Wrap(rate).Delete();

            EnsureModified();
        }

        public WorkExperienceItem AddWorkExperience()
        {
            WorkExperienceItem workExperienceItem = WorkExperienceItem.Create();
            WorkExperience.Add(workExperienceItem);

            EnsureModified();
            return workExperienceItem;
        }

        public void DeleteWorkExperience(WorkExperienceItem workExperienceItem)
        {
            WorkExperience.Remove(workExperienceItem);
            EntityAspect.Wrap(workExperienceItem).Delete();

            EnsureModified();
        }

        public Skill AddSkill()
        {
            Skill skill = Skill.Create();
            Skills.Add(skill);

            EnsureModified();
            return skill;
        }

        public void DeleteSkill(Skill skill)
        {
            Skills.Remove(skill);
            EntityAspect.Wrap(skill).Delete();

            EnsureModified();
        }

        private void EnsureModified()
        {
            if (!EntityAspect.Wrap(this).IsChanged) EntityAspect.Wrap(this).SetModified();
        }

        [AfterSet("FirstName")]
        public void AfterSetFirstName(object value)
        {
            EntityAspect.Wrap(this).ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }

        [AfterSet("MiddleName")]
        public void AfterSetMiddleName(object value)
        {
            EntityAspect.Wrap(this).ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }

        [AfterSet("LastName")]
        public void AfterSetLastName(object value)
        {
            EntityAspect.Wrap(this).ForcePropertyChanged(new PropertyChangedEventArgs("FullName"));
        }
    }
}