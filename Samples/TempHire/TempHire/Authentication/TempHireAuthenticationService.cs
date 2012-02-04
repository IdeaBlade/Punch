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

using System.ComponentModel.Composition;
using Cocktail;
using Common.Authentication;
using Security;
using Security.Messages;

namespace TempHire.Authentication
{
    [Export(typeof (IAuthenticationService))]
    [Export(typeof (IAuthenticationServiceEx))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TempHireAuthenticationService : AuthenticationService<SecurityEntities>, IAuthenticationServiceEx
    {
        #region IAuthenticationServiceEx Members

        public UserPrincipal CurrentUser
        {
            get { return Principal as UserPrincipal; }
        }

        #endregion

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