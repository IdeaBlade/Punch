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
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>Interface identifying an authentication service. If present, entity manager providers will use the authentication service to link the
    /// credentials between multiple entity managers.</summary>
    /// <example>
    /// 	<code title="Example" description="Demonstrates how to enable the authentication service in an application." lang="CS">
    /// public class AppBootstrapper : FrameworkBootstrapper&lt;MainFrameViewModel&gt;
    /// {
    ///     protected override void PrepareCompositionContainer(CompositionBatch batch)
    ///     {
    ///         base.PrepareCompositionContainer(batch);
    ///  
    ///         // Inject the authentication service into the framework.
    ///         batch.AddExportedValue&lt;IAuthenticationService&gt;(new AuthenticationService&lt;NorthwindIBEntities&gt;());
    ///     }
    /// }</code>
    /// </example>
    public interface IAuthenticationService : IHideObjectMembers
    {
        /// <summary>
        /// Returns the <see cref="IPrincipal"/> representing the current user.
        /// </summary>
        /// <value>Returns the current principal or null if not logged in.</value>
        IPrincipal Principal { get; }

        /// <summary>Returns whether the user is logged in.</summary>
        /// <value>Returns true if user is logged in.</value>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Returns the current DevForce AuthenticationContext.
        /// </summary>
        IAuthenticationContext AuthenticationContext { get; }

        /// <summary>
        /// Specifies the ConnectionOptions used by the current AuthenticationService.
        /// </summary>
        ConnectionOptions ConnectionOptions { get; }

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <param name="onSuccess">Callback called when login was successful.</param>
        /// <param name="onFail">Callback called when an error occurred during login.</param>
        OperationResult LoginAsync(ILoginCredential credential, Action onSuccess = null, Action<Exception> onFail = null);

        /// <summary>Logs out the current user.</summary>
        /// <param name="callback">Callback called when logout completes.</param>
        OperationResult LogoutAsync(Action callback = null);

#if !SILVERLIGHT

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <returns>A Boolean indicating success or failure.</returns>
        void Login(ILoginCredential credential);

        /// <summary>Logs out the current user.</summary>
        void Logout();

#endif

        /// <summary>Signals that a user successfully logged in.</summary>
        event EventHandler<EventArgs> LoggedIn;

        /// <summary>Signals that a user successfully logged out.</summary>
        event EventHandler<EventArgs> LoggedOut;

        /// <summary>
        /// Signals that the principal has changed due to a login or logout.
        /// </summary>
        event EventHandler<EventArgs> PrincipalChanged;
    }
}
