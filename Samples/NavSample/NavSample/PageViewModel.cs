using Caliburn.Micro;
using Cocktail;
using IdeaBlade.EntityModel;
using System.Collections.Generic;

namespace NavSample
{
    public abstract class PageViewModel : Screen, INavigationTarget
    {
        protected PageViewModel(INavigator navigator, ICustomerUnitOfWork unitOfWork, ErrorHandler errorHandler)
        {
            Navigator = navigator;
            UnitOfWork = unitOfWork;
            ErrorHandler = errorHandler;
        }

        public virtual void LoadState(object navigationParameter, Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            if (!UnitOfWork.IsRestored && sharedState.ContainsKey("uow"))
                UnitOfWork.Restore((EntityCacheState)sharedState["uow"]);

            IsRestored = pageState != null;
        }

        public virtual void OnNavigatedFrom(NavigationArgs args)
        {
        }

        public virtual void OnNavigatedTo(NavigationArgs args)
        {
        }

        public virtual void OnNavigatingFrom(NavigationCancelArgs args)
        {
        }

        public string PageKey { get; set; }

        public virtual void SaveState(Dictionary<string, object> pageState, Dictionary<string, object> sharedState)
        {
            sharedState["uow"] = UnitOfWork.GetCacheState();
        }

        protected INavigator Navigator { get; private set; }

        protected ICustomerUnitOfWork UnitOfWork { get; private set; }

        protected ErrorHandler ErrorHandler { get; private set; }

        protected bool IsRestored { get; private set; }
    }
}
