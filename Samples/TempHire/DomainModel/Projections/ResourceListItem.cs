using System;
using System.Runtime.Serialization;
using IdeaBlade.EntityModel;

namespace DomainModel.Projections
{
    [DataContract]
    public class ResourceListItem : IKnownType
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Zipcode { get; set; }

        [DataMember]
        public string AreaCode { get; set; }

        [DataMember]
        public string Number { get; set; }

        public string FullName
        {
            get
            {
                return !string.IsNullOrWhiteSpace(MiddleName)
                           ? string.Format("{0} {1} {2}", FirstName.Trim(), MiddleName.Trim(), LastName.Trim())
                           : string.Format("{0} {1}", FirstName.Trim(), LastName.Trim());
            }
        }

        public string PhoneNumber
        {
            get { return string.Format("({0}) {1}", AreaCode, Number); }
        }
    }
}