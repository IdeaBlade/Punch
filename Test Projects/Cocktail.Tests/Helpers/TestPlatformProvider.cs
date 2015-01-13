using Caliburn.Micro;
using System.Threading.Tasks;

namespace Cocktail.Tests.Helpers
{
    public class TestPlatformProvider : IPlatformProvider
    {
        private IPlatformProvider _defaultPlatformProvider = new DefaultPlatformProvider();

        public void BeginOnUIThread(System.Action action)
        {
            _defaultPlatformProvider.BeginOnUIThread(action);
        }

        public void ExecuteOnFirstLoad(object view, System.Action<object> handler)
        {
            _defaultPlatformProvider.ExecuteOnFirstLoad(view, handler);
        }

        public void ExecuteOnLayoutUpdated(object view, System.Action<object> handler)
        {
            _defaultPlatformProvider.ExecuteOnLayoutUpdated(view, handler);
        }

        public object GetFirstNonGeneratedView(object view)
        {
            return _defaultPlatformProvider.GetFirstNonGeneratedView(view);
        }

        public System.Action GetViewCloseAction(object viewModel, System.Collections.Generic.ICollection<object> views, bool? dialogResult)
        {
            return _defaultPlatformProvider.GetViewCloseAction(viewModel, views, dialogResult);
        }

        public bool InDesignMode
        {
            get { return false; }
        }

        public void OnUIThread(System.Action action)
        {
            _defaultPlatformProvider.OnUIThread(action);
        }

        public Task OnUIThreadAsync(System.Action action)
        {
            return _defaultPlatformProvider.OnUIThreadAsync(action);
        }
    }
}
