//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Security.Principal;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>Interface identifying an authentication service. If present, entity manager providers will use the authentication service to link the
    /// credentials between multiple entity managers.</summary>
    /// <example>
    /// 	<code title="Example" description="Demonstrates how to enable the authentication service in an application." lang="CS">
    /// public class AppBootstrapper : FrameworkBootstrapper&lt;MainFrameViewModel&gt;
    /// {
    ///     protected override void PrepareCompositionContainer(System.ComponentModel.Composition.Hosting.CompositionBatch batch)
    ///     {
    ///         base.PrepareCompositionContainer(batch);
    ///  
    ///         // Inject the authentication service into the framework.
    ///         batch.AddExportedValue&lt;IAuthenticationService&gt;(new AuthenticationService&lt;NorthwindIBEntities&gt;());
    ///     }
    /// }</code>
    /// </example>
    public interface IAuthenticationService : IAuthenticationManager
    {
        /// <summary>
        /// Returns the <see cref="IPrincipal"/> representing the current user.
        /// </summary>
        /// <value>Returns the current principal or null if not logged in.</value>
        IPrincipal Principal { get; }

        /// <summary>Returns whether the user is logged in.</summary>
        /// <value>Returns true if user is logged in.</value>
        bool IsLoggedIn { get; }

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <param name="onSuccess">Callback called when login was successful.</param>
        /// <param name="onFail">Callback called when an error occured during login.</param>
        INotifyCompleted LoginAsync(ILoginCredential credential, Action onSuccess = null, Action<Exception> onFail = null);

        /// <summary>Logs out the current user.</summary>
        /// <param name="onSuccess">Callback called when logout was successful.</param>
        /// <param name="onFail">Callback called when an error occured during logout.</param>
        INotifyCompleted LogoutAsync(Action onSuccess = null, Action<Exception> onFail = null);

#if !SILVERLIGHT

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <returns>A boolean indicating success or failure.</returns>
        bool Login(ILoginCredential credential);

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
