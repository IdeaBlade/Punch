using IdeaBlade.EntityModel;

namespace DomainModel
{
    public class TempHireEntities : EntityManager
    {
        public TempHireEntities(bool shouldConnect = true, string compositionContextName = null)
            : base(shouldConnect, compositionContextName: compositionContextName)
        {
        }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Resource entities. </summary>
        public EntityQuery<Resource> Resources { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Address entities. </summary>
        public EntityQuery<Address> Addresses { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for PhoneNumber entities. </summary>
        public EntityQuery<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for AddressType entities. </summary>
        public EntityQuery<AddressType> AddressTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for PhoneNumberType entities. </summary>
        public EntityQuery<PhoneNumberType> PhoneNumberTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Rate entities. </summary>
        public EntityQuery<Rate> Rates { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for RateType entities. </summary>
        public EntityQuery<RateType> RateTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for WorkExperienceItem entities. </summary>
        public EntityQuery<WorkExperienceItem> WorkExperienceItems { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Skill entities. </summary>
        public EntityQuery<Skill> Skills { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for State entities. </summary>
        public EntityQuery<State> States { get; set; }
    }
}