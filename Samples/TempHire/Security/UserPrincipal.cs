using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Security
{
    [DataContract]
    public class UserPrincipal : UserBase, IKnownType
    {
        public UserPrincipal(Guid id, IIdentity identity, IEnumerable<string> roles = null) : base(identity, roles)
        {
            Id = id;
        }

        [DataMember]
        public Guid Id { get; internal set; }
    }
}