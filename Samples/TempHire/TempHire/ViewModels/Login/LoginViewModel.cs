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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Factories;
using DomainServices.Repositories;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.Login
{
    [Export]
    public class LoginViewModel : Screen
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogManager _dialogManager;
        private readonly IGlobalCache _globalCache;
        private string _failureMessage;
        private IDialogUICommand<DialogResult> _loginCommand;
        private string _password;
        private string _username;
        private bool _cacheLoaded;

        [ImportingConstructor]
        public LoginViewModel(IAuthenticationService authenticationService, IDialogManager dialogManager,
                              [Import(AllowDefault = true)] IGlobalCache globalCache)
        {
            Busy = new BusyWatcher();
            _authenticationService = authenticationService;
            _dialogManager = dialogManager;
            _globalCache = globalCache;
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
                UpdateCommands();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                UpdateCommands();
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

        private bool CanLogin
        {
            get { return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
        }

        private IEnumerable<IResult> LoginAsync()
        {
            using (Busy.GetTicket())
            {
                FailureMessage = "";

                var credential = new LoginCredential(Username, Password, null);
                // Clear username and password fields
                Username = null;
                Password = null;

                OperationResult operation;
                yield return operation = _authenticationService.LoginAsync(credential).ContinueOnError();

                if (_authenticationService.IsLoggedIn)
                {
                    if (_globalCache != null)
                    {
                        yield return operation = _globalCache.LoadAsync().ContinueOnError();

                        _cacheLoaded = operation.CompletedSuccessfully;
                        if (operation.HasError)
                            FailureMessage = "Failed to load global entity cache. Try again!";
                    }
                }

                if (operation.HasError)
                    FailureMessage = operation.Error.Message;
            }
        }

        public DialogOperationResult<DialogResult> ShowAsync()
        {
            var commands = new List<IDialogUICommand<DialogResult>>();
            _loginCommand = new DialogUICommand<DialogResult>("Login", DialogResult.Ok, true);
            _loginCommand.Invoked += (sender, args) =>
            {
                args.Cancel(); // Cancel command, we'll take it from here.

                LoginAsync().ToSequentialResult().Execute(
                    _ =>
                        {
                            if (_authenticationService.IsLoggedIn && _cacheLoaded)
                                args.DialogHost.TryClose(_loginCommand.DialogResult);
                        });
            };
            commands.Add(_loginCommand);

#if !SILVERLIGHT
            var closeCommand = new DialogUICommand<DialogResult>("Close", DialogResult.Cancel, false, true);
            commands.Add(closeCommand);
#endif

            UpdateCommands();
            return _dialogManager.ShowDialogAsync(commands, this);
        }

        private void UpdateCommands()
        {
            _loginCommand.Enabled = CanLogin;
        }
    }

    [Export(typeof (IPartFactory<LoginViewModel>))]
    public class LoginViewModelFactory : PartFactoryBase<LoginViewModel>
    {
    }
}