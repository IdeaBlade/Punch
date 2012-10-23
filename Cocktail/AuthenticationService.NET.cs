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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;

namespace Cocktail
{
    public partial class AuthenticationService
    {
        #region IAuthenticationService Members

        /// <summary>
        ///   Login with the supplied credential.
        /// </summary>
        /// <param name="credential"> <para>The supplied credential.</para> </param>
        public void Login(ILoginCredential credential)
        {
            // Logout before logging in with new set of credentials
            if (IsLoggedIn) Logout();

            _authenticationContext = Authenticator.Instance.Login(credential, ConnectionOptions.ToLoginOptions());
            OnPrincipalChanged();
            OnLoggedIn();
        }

        /// <summary>
        ///   Logs out the current user.
        /// </summary>
        public void Logout()
        {
            if (!IsLoggedIn) return;

            try
            {
                Authenticator.Instance.Logout(_authenticationContext);
            }
            catch (Exception e)
            {
                // Ignoring error. It doesn't matter if the logout didn't go through to the server.
                TraceFns.WriteLine(string.Format(StringResources.LogoutFailed, e.Message));
            }
            OnPrincipalChanged();
            OnLoggedOut();
        }

        #endregion
    }
}