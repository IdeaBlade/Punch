using System;
using System.Security.Principal;
using Cocktail;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace TempHire.Authentication
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        #region IAuthenticationService Members

        public bool LinkAuthentication(EntityManager targetEm)
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

        public OperationResult LoginAsync(ILoginCredential credential, Action onSuccess = null,
                                           Action<Exception> onFail = null)
        {
            if (LoggedIn != null) 
                LoggedIn(this, EventArgs.Empty);
            if (PrincipalChanged != null) 
                PrincipalChanged(this, EventArgs.Empty);

            return AlwaysCompletedOperationResult.Instance;
        }

        public OperationResult LogoutAsync(Action onSuccess = null, Action<Exception> onFail = null)
        {
            if (LoggedOut != null)
                LoggedOut(this, EventArgs.Empty);
            if (PrincipalChanged != null)
                PrincipalChanged(this, EventArgs.Empty);

            return AlwaysCompletedOperationResult.Instance;
        }

        public event EventHandler<EventArgs> LoggedIn;
        public event EventHandler<EventArgs> LoggedOut;
        public event EventHandler<EventArgs> PrincipalChanged;

        #endregion
    }
}