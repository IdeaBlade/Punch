using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Security;
using Security.Messages;

namespace TempHire.Authentication
{
    [Export(typeof (IAuthenticationService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class TempHireAuthenticationService : AuthenticationService<SecurityEntities>
    {
        protected override void OnLoggedIn()
        {
            base.OnLoggedIn();
            EventFns.Publish(new LoggedInMessage(Principal));
        }

        protected override void OnLoggedOut()
        {
            base.OnLoggedOut();
            EventFns.Publish(new LoggedOutMessage());
        }
    }
}