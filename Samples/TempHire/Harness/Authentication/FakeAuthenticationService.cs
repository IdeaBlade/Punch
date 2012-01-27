//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

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