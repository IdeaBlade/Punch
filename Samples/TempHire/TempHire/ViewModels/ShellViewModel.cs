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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common.Toolbar;
using Common.Workspace;
using IdeaBlade.Core;
using Security.Messages;
using TempHire.ViewModels.Login;

namespace TempHire.ViewModels
{
    [Export]
    public class ShellViewModel : Conductor<object>, IDiscoverableViewModel, IHarnessAware,
                                  IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ExportFactory<LoginViewModel> _loginFactory;
        private readonly INavigator _navigator;
        private readonly IEnumerable<IWorkspace> _workspaces;

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<IWorkspace> workspaces, IToolbarManager toolbar,
                              IAuthenticationService authenticationService, ExportFactory<LoginViewModel> loginFactory)
        {
            Toolbar = toolbar;
            _workspaces = workspaces;
            _authenticationService = authenticationService;
            _loginFactory = loginFactory;
            _navigator = new Navigator(this);
        }

        public IToolbarManager Toolbar { get; private set; }

        public bool IsLoggedIn
        {
            get { return _authenticationService.IsLoggedIn; }
        }

        #region IHandle<LoggedInMessage> Members

        public void Handle(LoggedInMessage message)
        {
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        #endregion

        #region IHandle<LoggedOutMessage> Members

        public void Handle(LoggedOutMessage message)
        {
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        #endregion

        #region IHarnessAware Members

        /// <summary>
        ///   Provides the setup logic to be run before the ViewModel is activated inside of the development harness.
        /// </summary>
        public void Setup()
        {
#if HARNESS
            Start();
#endif
        }

        #endregion

        public ShellViewModel Start()
        {
            StartAsync();
            return this;
        }

        private async void StartAsync()
        {
            var mainGroup = new ToolbarGroup(0);
            _workspaces.OrderBy(w => w.Sequence).ForEach(
                w => mainGroup.Add(new ToolbarAction(this, w.DisplayName, async () => await NavigateToWorkspace(w))));

            var logoutGroup = new ToolbarGroup(100) { new ToolbarAction(this, "Logout", Logout) };

            Toolbar.Clear();
            Toolbar.AddGroup(mainGroup);
            Toolbar.AddGroup(logoutGroup);

            var home = GetHomeScreen();
            if (home != null)
                await NavigateToWorkspace(home);
        }

        public async Task Login()
        {
            await _loginFactory.CreateExport().Value.ShowAsync();

#if !SILVERLIGHT
            if (!_authenticationService.IsLoggedIn)
                TryClose();
#endif
        }

        public async void Logout()
        {
            var home = GetHomeScreen();
            LogFns.DebugWriteLineIf(home == null, "No workspace marked as default.");
            if (home == null)
                return;

            await NavigateToWorkspace(home);

            await _authenticationService.LogoutAsync();

            await Login();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();

#if !SILVERLIGHT
            DisplayName = "TempHire for WPF";
#endif
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // Launch login dialog
            await Login();
        }

        private IWorkspace GetHomeScreen()
        {
            return _workspaces.FirstOrDefault(w => w.IsDefault);
        }

        private async Task NavigateToWorkspace(IWorkspace workspace)
        {
            // Break if the workspace is already active.
            if (ActiveItem != null && ActiveItem.GetType() == workspace.ViewModelType)
                return;

            await _navigator.NavigateToAsync(workspace.ViewModelType);
        }
    }
}