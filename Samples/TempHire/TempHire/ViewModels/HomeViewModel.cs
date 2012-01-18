using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Workspace;

namespace TempHire.ViewModels
{
    [Export]
    public class HomeViewModel : Screen, IWorkspace, IDiscoverableViewModel
    {
        public HomeViewModel()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Home";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        #region IWorkspace Members

        public bool IsDefault
        {
            get { return true; }
        }

        public int Sequence
        {
            get { return 0; }
        }

        #endregion
    }
}