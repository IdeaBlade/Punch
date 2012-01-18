using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Caliburn.Micro;
using Cocktail;
using Common.BusyWatcher;
using Common.Errors;
using Common.Repositories;
using IdeaBlade.EntityModel;
using Security;

namespace TempHire.ViewModels.Login
{
    [Export]
    public class LoginViewModel : Screen, IResult
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IErrorHandler _errorHandler;
        private readonly ILookupRepository _lookupRepository;
        private readonly IWindowManager _windowManager;
        private string _failureMessage;
        private string _password;

        private string _username;

        [ImportingConstructor]
        public LoginViewModel(IAuthenticationService authenticationService, IWindowManager windowManager,
                              [Import(AllowDefault = true)] ILookupRepository lookupRepository,
                              [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IBusyWatcher busy,
                              IErrorHandler errorHandler)
        {
            Busy = busy;
            _authenticationService = authenticationService;
            _windowManager = windowManager;
            _lookupRepository = lookupRepository;
            _errorHandler = errorHandler;
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "";
// ReSharper restore DoNotCallOverridableMethodsInConstructor

#if DEBUG
            _username = "Admin";
            _password = "password";
#endif
        }

        public IBusyWatcher Busy { get; private set; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
            set
            {
                _failureMessage = value;
                NotifyOfPropertyChange(() => FailureMessage);
                NotifyOfPropertyChange(() => FailureMessageVisible);
            }
        }

        public bool FailureMessageVisible
        {
            get { return !string.IsNullOrWhiteSpace(_failureMessage); }
        }

        public bool CanLogin
        {
            get { return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        #region IResult Members

        public void Execute(ActionExecutionContext context)
        {
            _windowManager.ShowDialog(this);
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        #endregion

        public IEnumerable<IResult> Login()
        {
            using (Busy.GetTicket())
            {
                FailureMessage = "";

                var hash = CryptoHelper.GenerateKey(Password);
                var password = Encoding.UTF8.GetString(hash, 0, hash.Length);
                var credential = new LoginCredential(Username, password, null);
                // Clear username and password fields
                Username = null;
                Password = null;

                yield return _authenticationService.LoginAsync(
                    credential, onFail: e => FailureMessage = e.Message);

                if (_authenticationService.IsLoggedIn)
                {
                    if (_lookupRepository != null)
                        yield return _lookupRepository.InitializeAsync(onFail: _errorHandler.HandleError);

                    TryClose();
                }
            }
        }

        private void OnComplete()
        {
            if (Completed == null) return;

            var args = new ResultCompletionEventArgs();
            EventFns.RaiseOnce(ref Completed, this, args);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                OnComplete();
        }
    }
}