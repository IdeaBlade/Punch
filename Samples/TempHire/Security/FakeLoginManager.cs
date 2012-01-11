using System.ComponentModel.Composition;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Security
{
    /// <summary>
    /// LoginManager used when using the fake backing store. Allows for null credentials
    /// </summary>
    [PartNotDiscoverable]
    public class FakeLoginManager : LoginManager
    {
        public override IPrincipal Login(ILoginCredential credential, EntityManager entityManager)
        {
            if (credential == null)
                return new UserBase(new UserIdentity("fake", "FAKE", true));

            return base.Login(credential, entityManager);
        }
    }
}