using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.BusyWatcher;
using Common.Toolbar;
using Common.Workspace;
using IdeaBlade.Core;
using Security.Messages;
using TempHire.ViewModels.Login;

namespace TempHire.ViewModels
{
    [Export]
    public class ShellViewModel : Conductor<IWorkspace>, IDiscoverableViewModel, IHandle<LoggedInMessage>,
                                  IHandle<LoggedOutMessage>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ExportFactory<LoginViewModel> _loginFactory;
        private readonly IEnumerable<IWorkspace> _workspaces;
        private ToolbarGroup _toolbarGroup;

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<IWorkspace> workspaces, IToolbarManager toolbar,
                              IAuthenticationService authenticationService, ExportFactory<LoginViewModel> loginFactory,
                              [Import(RequiredCreationPolicy = CreationPolicy.Shared)] IBusyWatcher busy)
        {
            Toolbar = toolbar;
            Busy = busy;
            _workspaces = workspaces;
            _authenticationService = authenticationService;
            _loginFactory = loginFactory;

            EventFns.Subscribe(this);
        }

        public IToolbarManager Toolbar { get; private set; }
        public IBusyWatcher Busy { get; private set; }

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

        public ShellViewModel Start()
        {
            if (_toolbarGroup == null)
            {
                _toolbarGroup = new ToolbarGroup(0);
                _workspaces.OrderBy(w => w.Sequence).ForEach(
                    w => _toolbarGroup.Add(new ToolbarAction(this, w.DisplayName, () => NavigateTo(w))));
            }

            IWorkspace @default = _workspaces.FirstOrDefault(w => w.IsDefault);
            if (@default != null)
                NavigateTo(@default).ToSequentialResult().Execute();

            return this;
        }

        protected IEnumerable<IResult> NavigateTo(IWorkspace workspace)
        {
            yield return new NavigateResult<IWorkspace>(this, () => workspace);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Toolbar.AddGroup(_toolbarGroup);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Toolbar.RemoveGroup(_toolbarGroup);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // Launch login dialog
            LoginViewModel login = _loginFactory.CreateExport().Value;
            login.Execute();
        }
    }
}