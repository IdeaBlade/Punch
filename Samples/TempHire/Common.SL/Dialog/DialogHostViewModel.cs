using System;
using Caliburn.Micro;
using Caliburn.Micro.Extensions;

namespace Common.Dialog
{
    public class DialogHostViewModel : Conductor<object>
    {
        private IDialogHostDelegate _dialogHostDelegate;
        private DialogResult _dialogResult;

        private bool _isCancelVisible;

        public bool CanOk
        {
            get { return _dialogHostDelegate == null || _dialogHostDelegate.IsComplete; }
        }

        public bool IsCancelVisible
        {
            get { return _isCancelVisible; }
            private set
            {
                _isCancelVisible = value;
                NotifyOfPropertyChange(() => IsCancelVisible);
            }
        }

        public DialogResult DialogResult
        {
            get { return _dialogResult; }
            private set
            {
                _dialogResult = value;
                if (_dialogHostDelegate != null)
                    _dialogHostDelegate.DialogResult = _dialogResult;
                NotifyOfPropertyChange(() => DialogResult);
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public DialogHostViewModel Start(string title, object content, bool hideCancel = false)
        {
            _dialogHostDelegate = content as IDialogHostDelegate;
            if (_dialogHostDelegate != null)
            {
                _dialogHostDelegate.CompleteChanged -= CompleteChanged;
                _dialogHostDelegate.CompleteChanged += CompleteChanged;
            }

            DisplayName = title;
            IsCancelVisible = !hideCancel;
            DialogResult = DialogResult.Cancel;

            ActivateItem(content);
            return this;
        }

        private void CompleteChanged(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => CanOk);
        }

        public void Ok()
        {
            DialogResult = DialogResult.Ok;
            TryClose();
        }

        public void Cancel()
        {
            DialogResult = DialogResult.Cancel;
            TryClose();
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                OnComplete();
                if (_dialogHostDelegate != null)
                    _dialogHostDelegate.CompleteChanged -= CompleteChanged;

                ActiveItem = null;
                _dialogHostDelegate = null;
            }

            base.OnDeactivate(close);
        }

        protected void OnComplete()
        {
            if (Completed == null) return;

            var args = new ResultCompletionEventArgs {WasCancelled = DialogResult == DialogResult.Cancel};
            EventFns.RaiseOnce(ref Completed, this, args);
        }
    }
}