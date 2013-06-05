using Cocktail;
using IdeaBlade.Core;

namespace NavSample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : CocktailMefWindowsStoreApplication
    {
        public App() : base(typeof (ListPageViewModel))
        {
            InitializeComponent();
            EnableAutomaticSessionRestore();
        }

        protected override void StartRuntime()
        {
            base.StartRuntime();

            IdeaBladeConfig.Instance.ObjectServer.RemoteBaseUrl = "http://localhost";
            IdeaBladeConfig.Instance.ObjectServer.ServerPort = 57209;
            IdeaBladeConfig.Instance.ObjectServer.ServiceName = "EntityService.svc";
        }
    }
}