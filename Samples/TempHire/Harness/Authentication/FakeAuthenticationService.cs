using System;
using System.Security.Principal;
using IdeaBlade.Application.Framework.Core.Authentication;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.EntityModel;

namespace TempHire.Authentication
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        #region IAuthenticationService Members

        public bool LinkAuthentication(EntityManager targetEM)
        {
            return false;
        }

        public IPrincipal Principal
        {
            get { return new UserBase(new UserIdentity("fake", "FAKE", true)); }
        }

        public bool IsLoggedIn
        {
            get { return true; }
        }

        public INotifyCompleted Login(ILoginCredential credential, Action onSuccess = null,
                                      Action<Exception> onFail = null)
        {
            return LoginAsync(credential, onSuccess, onFail);
        }

        public INotifyCompleted Logout(Action onSuccess = null, Action<Exception> onFail = null)
        {
            return LogoutAsync(onSuccess, onFail);
        }

        public INotifyCompleted LoginAsync(ILoginCredential credential, Action onSuccess = null,
                                           Action<Exception> onFail = null)
        {
            if (LoggedIn != null) 
                LoggedIn(this, EventArgs.Empty);
            if (PrincipalChanged != null) 
                PrincipalChanged(this, EventArgs.Empty);

            return AlwaysCompleted.Instance;
        }

        public INotifyCompleted LogoutAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            if (LoggedOut != null)
                LoggedOut(this, EventArgs.Empty);
            if (PrincipalChanged != null)
                PrincipalChanged(this, EventArgs.Empty);

            return AlwaysCompleted.Instance;
        }

        public event EventHandler<EventArgs> LoggedIn;
        public event EventHandler<EventArgs> LoggedOut;
        public event EventHandler<EventArgs> PrincipalChanged;

        #endregion
    }
}