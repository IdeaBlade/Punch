using System.Runtime.Serialization;
using IdeaBlade.EntityModel;

namespace Test.Model
{
    [DataContract]
    public class PageProjection : IKnownType
    {
        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string City { get; set; }
    }
}