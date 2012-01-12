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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Security.Principal;
using IdeaBlade.EntityModel;
using Action = System.Action;
using Coroutine = IdeaBlade.EntityModel.Coroutine;

namespace Cocktail
{
    /// <summary>Is used by DevForce to obtain the authentication service used by the application.</summary>
    [PartNotDiscoverable]
    public class AuthenticationManagerProvider : IAuthenticationProvider
    {
        private static readonly PartLocator<IAuthenticationService> AuthenticationServiceLocator
            = new PartLocator<IAuthenticationService>(CreationPolicy.Shared);

        #region Implementation of IAuthenticationProvider

        /// <summary>Returns the authentication service used by the Application.</summary>
        /// <returns></returns>
        public IAuthenticationManager GetAuthenticationManager()
        {
            return AuthenticationServiceLocator.GetPart();
        }

        #endregion
    }

    /// <summary>Default implementation of an authentication service. Subclass if different behavior is desired, otherwise use as-is.</summary>
    /// <typeparam name="T">The type of the main EntityManager that should be used to handle Login and Logout.</typeparam>
    /// <example>
    /// 	<code title="Example" description="Demonstrates how to enable the authentication service in an application. " lang="CS">
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
    public class AuthenticationService<T> : IAuthenticationService, INotifyPropertyChanged
        where T : EntityManager
    {
        private T _manager;
        private bool _isPersistenceLayerInitialized;

        /// <summary>Initializes a new instance.</summary>
        public AuthenticationService(T entityManager = null)
        {
            Manager = entityManager;
        }

        #region Implementation of IAuthenticationManager

        /// <summary>
        /// Called by an EntityManager to request authentication credentials.
        /// </summary>
        /// <param name="target">The EntityManager requesting credentials</param>
        /// <returns>
        /// True if the link was made
        /// </returns>
        public virtual bool LinkAuthentication(EntityManager target)
        {
            if (!Manager.IsLoggedIn) return false;

            target.LinkForAuthentication(Manager);
            return true;
        }

        #endregion

        #region Implementation of IAuthenticationService<T>

        /// <summary>
        /// Returns the <see cref="IPrincipal"/> representing the current user.
        /// </summary>
        /// <value>Returns the current principal or null if not logged in.</value>
        public virtual IPrincipal Principal
        {
            get { return Manager.Principal; }
        }

        /// <summary>Returns whether the user is logged in.</summary>
        /// <value>Returns true if user is logged in.</value>
        public virtual bool IsLoggedIn { get { return Manager.IsLoggedIn && Principal.Identity.IsAuthenticated; } }

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <param name="onSuccess">Callback called when login was successful.</param>
        /// <param name="onFail">Callback called when an error occured during login.</param>
        public OperationResult LoginAsync(ILoginCredential credential, Action onSuccess, Action<Exception> onFail)
        {
            CoroutineOperation coop = Coroutine.Start(
                () => LoginAsyncCore(credential),
                op =>
                {
                    if (op.CompletedSuccessfully)
                    {
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
            if (!_isPersistenceLayerInitialized)
            {
                yield return FakeBackingStoreManager.Instance.InitializeAllAsync();
                _isPersistenceLayerInitialized = FakeBackingStoreManager.Instance.IsInitialized;
            }

            // Logout before logging in with new set of credentials
            if (Manager.IsLoggedIn) yield return LogoutAsync(null, null);

            yield return Manager.LoginAsync(credential);
        }

        /// <summary>Logs out the current user.</summary>
        /// <param name="onSuccess">Callback called when logout was successful.</param>
        /// <param name="onFail">Callback called when an error occured during logout.</param>
        public OperationResult LogoutAsync(Action onSuccess, Action<Exception> onFail)
        {
            BaseOperation op = Manager.LogoutAsync();
            op.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    OnPrincipalChanged();
                    OnLoggedOut();
                    if (onSuccess != null) onSuccess();
                }

                if (args.HasError && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };

            return op.AsOperationResult();
        }

#if !SILVERLIGHT

        private void EnsurePersistenceLayerIsInitialized()
        {
            if (_isPersistenceLayerInitialized) return;

            FakeBackingStoreManager.Instance.InitializeAll();
            _isPersistenceLayerInitialized = FakeBackingStoreManager.Instance.IsInitialized;
        }

        /// <summary>Login with the supplied credential.</summary>
        /// <param name="credential">
        /// 	<para>The supplied credential.</para>
        /// </param>
        /// <returns>A boolean indicating success or failure.</returns>
        public bool Login(ILoginCredential credential)
        {
            EnsurePersistenceLayerIsInitialized();

            // Logout before logging in with new set of credentials
            if (Manager.IsLoggedIn) Logout();

            if (Manager.Login(credential))
            {
                OnPrincipalChanged();
                OnLoggedIn();
                return true;
            }
            return false;
        }

        /// <summary>Logs out the current user.</summary>
        public void Logout()
        {
            Manager.Logout();
            OnPrincipalChanged();
            OnLoggedOut();
        }

#endif

        #endregion

        /// <summary>Returns the EntityManager to be used for authentication</summary>
        protected T Manager
        {
            get
            {
                if (_manager != null) return _manager;

                var providerLocator = new PartLocator<IEntityManagerProvider<T>>(CreationPolicy.Any);
                var provider = (EntityManagerProviderBase<T>)providerLocator.GetPart();
                return _manager = provider.CreateAuthenticationManager();
            }
            private set { _manager = value; }
        }

        #region INotifyPropertyChanged Members

        /// <summary>Notifies of changed properties.</summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        /// <summary>Internal use.</summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Triggers the LoggedIn event.</summary>
        protected virtual void OnLoggedIn()
        {
            NotifyPropertyChanged("IsLoggedIn");
            LoggedIn(this, EventArgs.Empty);
        }

        /// <summary>Triggers the LoggedOut event.</summary>
        protected virtual void OnLoggedOut()
        {
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
    }
}