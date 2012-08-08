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

using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Test.Web
{
    public class LoginManager : IEntityLoginManager
    {
        #region Implementation of IEntityLoginManager

        /// <summary>
        /// Validate user credentials and return an IPrinicipal.
        /// </summary>
        /// <param name="credential">Credentials to validate</param><param name="entityManager">A server-side EntityManager</param>
        /// <remarks>
        /// <para>
        /// The <see cref="T:System.Security.Principal.IPrincipal"/> returned here is wrapped into a <see cref="T:IdeaBlade.EntityModel.SessionBundle"/>
        ///             before returning to the client-side code which called <see cref="M:IdeaBlade.EntityModel.EntityManager.Login(IdeaBlade.EntityModel.ILoginCredential)">EntityManager.Login</see>.
        ///             The <see cref="T:IdeaBlade.EntityModel.EntityManager"/> uses the SessionBundle in all further communications with the Entity Server.
        ///             The <b>IPrincipal</b> returned is also used to set the <see cref="P:System.Threading.Thread.CurrentPrincipal"/>
        ///             on the client and on the Entity Server during query and save processing.
        /// </para>
        /// <para>
        /// <see cref="T:System.Security.Principal.GenericPrincipal"/> and <see cref="T:System.Security.Principal.WindowsPrincipal"/>
        ///             are two standard implementations of the <b>IPrincipal</b> interface, but you may also use custom implementations.
        ///             The <b>UserBase</b> type is also provided for Silverlight and ASP.NET applications.
        /// </para>
        /// <para>
        /// The <paramref name="entityManager"/> passed to this method is a special server-side EntityManager which is
        ///             "connected" to the EntityServer and which does not require login credentials.  You can use this EntityManager to query
        ///             your domain model; if necessary, you can create a domain-specific EntityManager from this EntityManager using
        ///             the <see cref="M:IdeaBlade.EntityModel.EntityManager.#ctor(IdeaBlade.EntityModel.EntityManager)"/>
        ///             constructor overload of your domain manager.
        /// </para>
        /// <para>
        /// Implementers should throw a <see cref="T:IdeaBlade.EntityModel.LoginException"/> if the credentials passed fail validation.
        /// </para>
        /// </remarks>
        public IPrincipal Login(ILoginCredential credential, EntityManager entityManager)
        {
            if (credential == null)
                return new UserBase(new UserIdentity("Anonymous"));

            return new UserBase(new UserIdentity(credential.UserName, "", true));
        }

        /// <summary>
        /// Called when a user logs out.
        /// </summary>
        /// <param name="principal">IPrincipal identifying the user</param><param name="entityManager">A server-side EntityManager</param>
        /// <remarks>
        /// Use <b>Logout</b> to perform any processing you require when a user logs out, such
        ///             as resource cleanup or auditing.
        ///             Note that you must implement this interface method even if you have no special logout
        ///             processing.
        /// </remarks>
        public void Logout(IPrincipal principal, EntityManager entityManager)
        {
        }

        #endregion
    }
}
