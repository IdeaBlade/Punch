// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.ComponentModel.Composition;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using Common.Security;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using Security;

namespace TempHire.Authentication
{
    [Export(typeof (IAuthenticationService))]
    [Export(typeof (IUserService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FakeAuthenticationService : IAuthenticationService, IUserService
    {
        #region IAuthenticationService Members

        public Task LoginAsync(ILoginCredential credential)
        {
            return LoginAsync(credential, CancellationToken.None);
        }

        public Task LoginAsync(ILoginCredential credential, CancellationToken cancellationToken)
        {
            if (LoggedIn != null)
                LoggedIn(this, EventArgs.Empty);
            if (PrincipalChanged != null)
                PrincipalChanged(this, EventArgs.Empty);

            return OperationResult.FromResult(true);
        }

        public Task LogoutAsync()
        {
            return LogoutAsync(CancellationToken.None);
        }

        public Task LogoutAsync(CancellationToken cancellationToken)
        {
            if (LoggedOut != null)
                LoggedOut(this, EventArgs.Empty);
            if (PrincipalChanged != null)
                PrincipalChanged(this, EventArgs.Empty);

            return OperationResult.FromResult(true);
        }

        public IPrincipal Principal
        {
            get { return new UserPrincipal(Guid.Empty, new UserIdentity("fake", "FAKE", true)); }
        }

        public bool IsLoggedIn
        {
            get { return true; }
        }

        public IAuthenticationContext AuthenticationContext
        {
            get { return AnonymousAuthenticationContext.Instance; }
        }

        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.Fake; }
        }

        public event EventHandler<EventArgs> LoggedIn;
        public event EventHandler<EventArgs> LoggedOut;
        public event EventHandler<EventArgs> PrincipalChanged;

        #endregion

        #region IUserService Members

        public UserPrincipal CurrentUser
        {
            get { return Principal as UserPrincipal; }
        }

        #endregion

        public void Login(ILoginCredential credential)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}