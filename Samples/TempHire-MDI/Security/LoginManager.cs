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

using System.Linq;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Security
{
    /// <summary>
    /// Production login manager
    /// </summary>
    public class LoginManager : IEntityLoginManager
    {
        #region IEntityLoginManager Members

        public virtual IPrincipal Login(ILoginCredential credential, EntityManager entityManager)
        {
            if (credential == null)
                throw new LoginException(LoginExceptionType.NoCredentials, "Credentials are required.");

            if (string.IsNullOrWhiteSpace(credential.UserName))
                throw new LoginException(LoginExceptionType.InvalidUserName, "Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(credential.Password))
                throw new LoginException(LoginExceptionType.InvalidPassword, "Password cannot be empty.");

            var em = new SecurityEntities(entityManager);
            User user =
                em.Users.FirstOrDefault(
                    u => u.Username.ToUpper() == credential.UserName.ToUpper() && u.Password == credential.Password);

            if (user == null)
                throw new LoginException(LoginExceptionType.InvalidPassword, credential.Domain, credential.UserName);

            return new UserPrincipal(user.Id, new UserIdentity(user.Username, "FORM", true));
        }

        public virtual void Logout(IPrincipal principal, EntityManager entityManager)
        {
        }

        #endregion
    }
}