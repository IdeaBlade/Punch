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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;

namespace Cocktail
{
    /// <summary>Default implementation of an authentication service. Subclass if different behavior is desired, otherwise use as-is.</summary>
    /// <example>
    /// 	<code title="Example" description="Demonstrates how to enable the authentication service in an application. " lang="CS">
    /// public class AppBootstrapper : FrameworkBootstrapper&lt;MainFrameViewModel&gt;
    /// {
    ///     protected override void PrepareCompositionContainer(CompositionBatch batch)
    ///     {
    ///         base.PrepareCompositionContainer(batch);
    ///  
    ///         // Inject the authentication service into the framework.
    ///         batch.AddExportedValue&lt;IAuthenticationService&gt;(new AuthenticationService());
    ///     }
    /// }</code>
    /// </example>
    public class AuthenticationService : IAuthenticationService, IAuthenticationContext, INotifyPropertyChanged,
                                         ICloneable
    {
        private string _connectionsOptionsName;
        private IAuthenticationContext _authenticationContext;

        /// <summary>
        /// Creates a new AuthenticationService instance.
        /// </summary>
        public AuthenticationService()
        {
            _authenticationContext = LoggedOutAuthenticationContext.Instance;
        }

        #region Implementation of IAuthenticationService

        Guid IAuthenticationContext.SessionKey
        {
            get { return _authenticationContext.SessionKey; }
        }

        /// <summary>
        /// Returns the <see cref="IPrincipal"/> representing the current user.
        /// </summary>
        /// <value>Returns the current principal or null if not logged in.</value>
        public virtual IPrincipal Principal
        {
            get { return _authenticationContext.Principal; }
        }

        LoginState IAuthenticationContext.LoginState
        {
            get { return _authenticationContext.LoginState; }
        }

        IDictionary<string, object> IAuthenticationContext.ExtendedPropertyMap
        {
            get { return _authenticationContext.ExtendedPropertyMap; }
        }

        /// <summary>Returns whether the user is logged in.</summary>
        /// <value>Returns true if user is logged in.</value>
        public virtual bool IsLoggedIn
        {
            get
            {
                return _authenticationContext.LoginState == LoginState.LoggedIn &&
                       Principal.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Returns the current DevForce AuthenticationContext.
        /// </summary>
        public IAuthenticationContext AuthenticationContext
        {
            get { return this; }
        }

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <param name="onSuccess">Callback called when login was successful.</param>
        /// <param name="onFail">Callback called when an error occurred during login.</param>
        public OperationResult LoginAsync(ILoginCredential credential, Action onSuccess = null,
                                          Action<Exception> onFail = null)
        {
            CoroutineOperation coop = Coroutine.Start(
                () => LoginAsyncCore(credential),
                op =>
                    {
                        if (op.CompletedSuccessfully)
                        {
                            _authenticationContext = (IAuthenticationContext) op.Result;
                            OnPrincipalChanged();
                            OnLoggedIn();
                        }
                        op.OnComplete(onSuccess, onFail);
                    });

            return coop.AsOperationResult();
        }

        /// <summary>Internal use.</summary>
        /// <param name="credential">The user's credentials.</param>
        protected virtual IEnumerable<INotifyCompleted> LoginAsyncCore(ILoginCredential credential)
        {
            // Logout before logging in with new set of credentials
            if (IsLoggedIn) yield return LogoutAsync();

            LoginOperation operation;
            yield return operation = Authenticator.Instance.LoginAsync(credential, ConnectionOptions.ToLoginOptions());

            yield return Coroutine.Return(operation.AuthenticationContext);
        }

        /// <summary>Logs out the current user.</summary>
        /// <param name="callback">Callback called when logout completes.</param>
        public OperationResult LogoutAsync(Action callback = null)
        {
            if (!IsLoggedIn)
            {
                if (callback != null) callback();
                return AlwaysCompletedOperationResult.Instance;
            }

            BaseOperation op = Authenticator.Instance.LogoutAsync(_authenticationContext);
            op.Completed += (s, args) =>
                                {
                                    // Ignore the error. We don't care if the logout couldn't reach the server.
                                    if (args.HasError)
                                        args.MarkErrorAsHandled();

                                    OnPrincipalChanged();
                                    OnLoggedOut();
                                    if (callback != null) callback();
                                };

            return op.AsOperationResult();
        }

#if !SILVERLIGHT

    /// <summary>Login with the supplied credential.</summary>
    /// <param name="credential">
    /// 	<para>The supplied credential.</para>
    /// </param>
    /// <returns>A Boolean indicating success or failure.</returns>
        public void Login(ILoginCredential credential)
        {
            // Logout before logging in with new set of credentials
            if (IsLoggedIn) Logout();

            _authenticationContext = Authenticator.Instance.Login(credential, ConnectionOptions.ToLoginOptions());
            OnPrincipalChanged();
            OnLoggedIn();
        }

        /// <summary>Logs out the current user.</summary>
        public void Logout()
        {
            if (!IsLoggedIn) return;

            try
            {
                Authenticator.Instance.Logout(_authenticationContext);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
                // Ignore error. We don't care if the logout doesn't reach the server.
            }
            OnPrincipalChanged();
            OnLoggedOut();
        }

#endif

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>Notifies of changed properties.</summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        /// <summary>
        /// Creates a new AuthenticationService from the current AuthenticationService and assigns the specified <see cref="ConnectionOptions"/> name.
        /// </summary>
        /// <param name="connectionOptionsName">The <see cref="ConnectionOptions"/> name to be assigned.</param>
        /// <returns>A new AuthenticationService instance.</returns>
        public AuthenticationService With(string connectionOptionsName)
        {
            var authenticationService = (AuthenticationService) ((ICloneable) this).Clone();
            authenticationService._connectionsOptionsName = connectionOptionsName;
            return authenticationService;
        }

        /// <summary>
        /// Specifies the <see cref="IEntityManagerProvider.ConnectionOptions"/> used by the current AuthenticationService.
        /// </summary>
        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.GetByName(_connectionsOptionsName); }
        }

        /// <summary>Internal use.</summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Triggers the LoggedIn event.</summary>
        protected virtual void OnLoggedIn()
        {
            DebugFns.WriteLine(string.Format(StringResources.SuccessfullyLoggedIn, ConnectionOptions.Name,
                                             ConnectionOptions.IsFake));

            NotifyPropertyChanged("IsLoggedIn");
            LoggedIn(this, EventArgs.Empty);
        }

        /// <summary>Triggers the LoggedOut event.</summary>
        protected virtual void OnLoggedOut()
        {
            _authenticationContext = LoggedOutAuthenticationContext.Instance;
            NotifyPropertyChanged("IsLoggedIn");
            LoggedOut(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the PrincipalChanged event.
        /// </summary>
        protected virtual void OnPrincipalChanged()
        {
            NotifyPropertyChanged("Principal");
            PrincipalChanged(this, EventArgs.Empty);
        }

        /// <summary>Signals that a user successfully logged in.</summary>
        public event EventHandler<EventArgs> LoggedIn = delegate { };

        /// <summary>Signals that a user successfully logged out.</summary>
        public event EventHandler<EventArgs> LoggedOut = delegate { };

        /// <summary>
        /// Signals that the principal has changed due to a login or logout.
        /// </summary>
        public event EventHandler<EventArgs> PrincipalChanged = delegate { };

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }

    internal class LoggedOutAuthenticationContext : IAuthenticationContext
    {
        private static LoggedOutAuthenticationContext _instance;
        private readonly IDictionary<string, object> _extendedPropertyMap;

        protected LoggedOutAuthenticationContext()
        {
            _extendedPropertyMap = new ReadOnlyDictionary<string, object>();
        }

        public static LoggedOutAuthenticationContext Instance
        {
            get { return _instance ?? (_instance = new LoggedOutAuthenticationContext()); }
        }

        #region IAuthenticationContext Members

        public Guid SessionKey
        {
            get { return Guid.Empty; }
        }

        public IPrincipal Principal
        {
            get { return null; }
        }

        public LoginState LoginState
        {
            get { return LoginState.LoggedOutMustLoginExplicitly; }
        }

        public IDictionary<string, object> ExtendedPropertyMap
        {
            get { return _extendedPropertyMap; }
        }

        #endregion
    }

    /// <summary>
    /// A singleton implementation of the AuthenticationContext for an anonymous user.
    /// </summary>
    public class AnonymousAuthenticationContext : IAuthenticationContext
    {
        private static AnonymousAuthenticationContext _instance;
        private readonly IDictionary<string, object> _extendedPropertyMap;
        private readonly IPrincipal _principal;
        private readonly Guid _sessionKey;

        /// <summary>
        /// Creates a new AnonymousAuthenticationContext.
        /// </summary>
        protected AnonymousAuthenticationContext()
        {
            _sessionKey = Guid.NewGuid();
            _principal = new UserBase(new UserIdentity("Anonymous"));
            _extendedPropertyMap = new ReadOnlyDictionary<string, object>();
        }

        /// <summary>
        /// Returns the current instance.
        /// </summary>
        public static AnonymousAuthenticationContext Instance
        {
            get { return _instance ?? (_instance = new AnonymousAuthenticationContext()); }
        }

        #region Implementation of IAuthenticationContext

        /// <summary>
        /// Token uniquely identifying a user session to the Entity Server.
        /// </summary>
        public Guid SessionKey
        {
            get { return _sessionKey; }
        }

        /// <summary>
        /// The <see cref="T:System.Security.Principal.IPrincipal"/> representing the logged in user.
        /// </summary>
        public IPrincipal Principal
        {
            get { return _principal; }
        }

        /// <summary>
        /// Returns whether this context is logged in.
        /// </summary>
        public LoginState LoginState
        {
            get { return LoginState.LoggedIn; }
        }

        /// <summary>
        /// Additional properties.
        /// </summary>
        public IDictionary<string, object> ExtendedPropertyMap
        {
            get { return _extendedPropertyMap; }
        }

        #endregion
    }

    internal class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _innerDictionary;

        public ReadOnlyDictionary()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(StringResources.DictionaryIsReadOnly);
        }

        public void Clear()
        {
            throw new NotSupportedException(StringResources.DictionaryIsReadOnly);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _innerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _innerDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(StringResources.DictionaryIsReadOnly);
        }

        public int Count
        {
            get { return _innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(TKey key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException(StringResources.DictionaryIsReadOnly);
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException(StringResources.DictionaryIsReadOnly);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _innerDictionary[key]; }
            set { throw new NotSupportedException(StringResources.DictionaryIsReadOnly); }
        }

        public ICollection<TKey> Keys
        {
            get { return _innerDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _innerDictionary.Values; }
        }
    }
}