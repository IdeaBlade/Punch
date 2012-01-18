using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

namespace SimplePopup.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class EnterNameViewModel : Screen, IDialogHostDelegate
    {
        private string _myName;

        public string MyName
        {
            get { return _myName; }
            set
            {
                _myName = value;
                NotifyOfPropertyChange(() => MyName);
                OnCompleteChanged();
            }
        }

        #region IDialogHostAware Members

        public bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(_myName); }
        }

        public DialogResult DialogResult { get; set; }

        public event EventHandler CompleteChanged = delegate { };

        #endregion

        private void OnCompleteChanged()
        {
            CompleteChanged(this, EventArgs.Empty);
        }

        public override void CanClose(Action<bool> callback)
        {
            if (DialogResult != DialogResult.Cancel)
            {
                callback(IsComplete);
            }
            else
                base.CanClose(callback);
        }
    }
}