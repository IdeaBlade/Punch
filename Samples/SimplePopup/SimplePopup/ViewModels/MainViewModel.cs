using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

namespace SimplePopup.ViewModels
{
    [Export]
    public class MainViewModel : Screen
    {
        private readonly IDialogManager _dialogManager;
        private readonly EnterNameViewModel _namePopup;

        [ImportingConstructor]
        public MainViewModel(IDialogManager dialogManager, EnterNameViewModel namePopup)
        {
            _dialogManager = dialogManager;
            _namePopup = namePopup;
        }

        public IEnumerable<IResult> SayHello()
        {
            yield return _dialogManager.ShowDialog(_namePopup, DialogButtons.OkCancel, "Name please");

            yield return _dialogManager.ShowMessage("Hello " + _namePopup.MyName);
        }
    }
}