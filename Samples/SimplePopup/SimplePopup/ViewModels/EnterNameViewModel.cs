using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;

namespace SimplePopup.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class EnterNameViewModel : Screen
    {
        private string _myName;
        private DialogButton _okButton;

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

        public bool IsComplete
        {
            get { return !string.IsNullOrWhiteSpace(_myName); }
        }

        private void OnCompleteChanged()
        {
            _okButton.Enabled = IsComplete;
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!this.DialogHost().DialogResult.Equals(DialogResult.Cancel))
            {
                callback(IsComplete);
            }
            else
                base.CanClose(callback);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _okButton = this.DialogHost().GetButton(DialogResult.Ok);
            _okButton.Enabled = IsComplete;
        }
    }
}