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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.EntityModel.Security;

namespace Cocktail
{
    /// <summary>
    ///   Interface used to configure the AuthenticationService
    /// </summary>
    public interface IAuthenticationServiceConfigurator : IHideObjectMembers
    {
        /// <summary>
        ///   Configures the name of the <see cref="ConnectionOptions" /> to be used.
        /// </summary>
        /// <param name="connectionOptionsName"> The name of the ConnectionOptions. </param>
        IAuthenticationServiceConfigurator WithConnectionOptions(string connectionOptionsName);
    }

    /// <summary>
    ///   Default implementation of an authentication service. Subclass if different behavior is desired, otherwise use as-is.
    /// </summary>
    /// <example>
    ///   <code title="Example" description="Demonstrates how to enable the authentication service in an application. " lang="CS">public class AppBootstrapper : FrameworkBootstrapper&lt;MainFrameViewModel&gt;
    ///     {
    ///     protected override void PrepareCompositionContainer(CompositionBatch batch)
    ///     {
    ///     base.PrepareCompositionContainer(batch);
    ///  
    ///     // Inject the authentication service into the framework.
    ///     batch.AddExportedValue&lt;IAuthenticationService&gt;(new AuthenticationService());
    ///     }
    ///     }</code>
    /// </example>
    public partial class AuthenticationService : IAuthenticationService, IAuthenticationContext, INotifyPropertyChanged
    {
        private readonly AuthenticationServiceConfiguration _configuration;
        private IAuthenticationContext _authenticationContext;

        /// <summary>
        ///   Creates a new AuthenticationService instance.
        /// </summary>
        public AuthenticationService()
        {
            _configuration = new AuthenticationServiceConfiguration();
            _authenticationContext = LoggedOutAuthenticationContext.Instance;
        }

        #region Implementation of IAuthenticationService

        #region IAuthenticationContext Members

        Guid IAuthenticationContext.SessionKey
        {
            get { return _authenticationContext.SessionKey; }
        }

        LoginState IAuthenticationContext.LoginState
        {
            get { return _authenticationContext.LoginState; }
        }

        IDictionary<string, object> IAuthenticationContext.ExtendedPropertyMap
        {
            get { return _authenticationContext.ExtendedPropertyMap; }
        }

        #endregion

        #region IAuthenticationService Members

        /// <summary>
        ///   Returns the <see cref="IPrincipal" /> representing the current user.
        /// </summary>
        /// <value> Returns the current principal or null if not logged in. </value>
        public virtual IPrincipal Principal
        {
            get { return _authenticationContext.Principal; }
        }

        /// <summary>
        ///   Returns whether the user is logged in.
        /// </summary>
        /// <value> Returns true if user is logged in. </value>
        public virtual bool IsLoggedIn
        {
            get
            {
                return _authenticationContext.LoginState == LoginState.LoggedIn &&
                       Principal.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        ///   Returns the current DevForce AuthenticationContext.
        /// </summary>
        public IAuthenticationContext AuthenticationContext
        {
            get { return this; }
        }

        /// <summary>
        ///   Login with the supplied credential.
        /// </summary>
        /// <param name="credential">
        ///   <para> The supplied credential. </para>
        /// </param>
        public Task LoginAsync(ILoginCredential credential)
        {
            return LoginAsync(credential, CancellationToken.None);
        }

        /// <summary>
        ///   Login with the supplied credential.
        /// </summary>
        /// <param name="credential"> The supplied credential. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        public virtual async Task LoginAsync(ILoginCredential credential, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Logout before logging in with new set of credentials
            if (IsLoggedIn) await LogoutAsync();

            _authenticationContext = await Authenticator.Instance.LoginAsync(
                credential, ConnectionOptions.ToLoginOptions(), cancellationToken);
            OnPrincipalChanged();
            OnLoggedIn();
        }

        /// <summary>
        ///   Logs out the current user.
        /// </summary>
        public Task LogoutAsync()
        {
            return LogoutAsync(CancellationToken.None);
        }

        /// <summary>
        ///   Logs out the current user.
        /// </summary>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        public async Task LogoutAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsLoggedIn) return;

            try
            {
                await Authenticator.Instance.LogoutAsync(_authenticationContext, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                throw;
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

        #endregion

        #region IAuthenticationService Members

        /// <summary>
        ///   Specifies the <see cref="IEntityManagerProvider.ConnectionOptions" /> used by the current AuthenticationService.
        /// </summary>
        public ConnectionOptions ConnectionOptions
        {
            get { return ConnectionOptions.GetByName(_configuration.ConnectionOptionsName); }
        }

        /// <summary>
        ///   Signals that a user successfully logged in.
        /// </summary>
        public event EventHandler<EventArgs> LoggedIn = delegate { };

        /// <summary>
        ///   Signals that a user successfully logged out.
        /// </summary>
        public event EventHandler<EventArgs> LoggedOut = delegate { };

        /// <summary>
        ///   Signals that the principal has changed due to a login or logout.
        /// </summary>
        public event EventHandler<EventArgs> PrincipalChanged = delegate { };

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        ///   Notifies of changed properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        /// <summary>
        ///   Configures the current AuthenticationService.
        /// </summary>
        /// <param name="configure"> Delegate to perform the configuration. </param>
        public AuthenticationService Configure(Action<IAuthenticationServiceConfigurator> configure)
        {
            configure(_configuration);
            return this;
        }

        /// <summary>
        ///   Internal use.
        /// </summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///   Triggers the LoggedIn event.
        /// </summary>
        protected virtual void OnLoggedIn()
        {
            DebugFns.WriteLine(string.Format(StringResources.SuccessfullyLoggedIn, ConnectionOptions.Name,
                                             ConnectionOptions.IsFake));

            NotifyPropertyChanged("IsLoggedIn");
            LoggedIn(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Triggers the LoggedOut event.
        /// </summary>
        protected virtual void OnLoggedOut()
        {
            _authenticationContext = LoggedOutAuthenticationContext.Instance;
            NotifyPropertyChanged("IsLoggedIn");
            LoggedOut(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Triggers the PrincipalChanged event.
        /// </summary>
        protected virtual void OnPrincipalChanged()
        {
            NotifyPropertyChanged("Principal");
            PrincipalChanged(this, EventArgs.Empty);
            EventFns.Publish(new PrincipalChangedMessage(Principal));
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
    ///   A singleton implementation of the AuthenticationContext for an anonymous user.
    /// </summary>
    public class AnonymousAuthenticationContext : IAuthenticationContext
    {
        private static AnonymousAuthenticationContext _instance;
        private readonly IDictionary<string, object> _extendedPropertyMap;
        private readonly IPrincipal _principal;
        private readonly Guid _sessionKey;

        /// <summary>
        ///   Creates a new AnonymousAuthenticationContext.
        /// </summary>
        protected AnonymousAuthenticationContext()
        {
            _sessionKey = Guid.NewGuid();
            _principal = new UserBase(new UserIdentity("Anonymous"));
            _extendedPropertyMap = new ReadOnlyDictionary<string, object>();
        }

        /// <summary>
        ///   Returns the current instance.
        /// </summary>
        public static AnonymousAuthenticationContext Instance
        {
            get { return _instance ?? (_instance = new AnonymousAuthenticationContext()); }
        }

        #region Implementation of IAuthenticationContext

        /// <summary>
        ///   Token uniquely identifying a user session to the Entity Server.
        /// </summary>
        public Guid SessionKey
        {
            get { return _sessionKey; }
        }

        /// <summary>
        ///   The <see cref="T:System.Security.Principal.IPrincipal" /> representing the logged in user.
        /// </summary>
        public IPrincipal Principal
        {
            get { return _principal; }
        }

        /// <summary>
        ///   Returns whether this context is logged in.
        /// </summary>
        public LoginState LoginState
        {
            get { return LoginState.LoggedIn; }
        }

        /// <summary>
        ///   Additional properties.
        /// </summary>
        public IDictionary<string, object> ExtendedPropertyMap
        {
            get { return _extendedPropertyMap; }
        }

        #endregion
    }

    internal class AuthenticationServiceConfiguration : IAuthenticationServiceConfigurator
    {
        public string ConnectionOptionsName { get; private set; }

        #region IAuthenticationServiceConfigurator Members

        public IAuthenticationServiceConfigurator WithConnectionOptions(string connectionOptionsName)
        {
            ConnectionOptionsName = connectionOptionsName;
            return this;
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

        #region IDictionary<TKey,TValue> Members

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

        #endregion
    }
}