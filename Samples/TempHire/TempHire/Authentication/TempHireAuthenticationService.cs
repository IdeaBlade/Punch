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
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public TempHireAuthenticationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override void OnLoggedIn()
        {
            base.OnLoggedIn();
            _eventAggregator.Publish(new LoggedInMessage(Principal));
        }

        protected override void OnLoggedOut()
        {
            base.OnLoggedOut();
            _eventAggregator.Publish(new LoggedOutMessage());
        }
    }
}