//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>Specifies constants defining which buttons to display on a dialog or message box.</summary>
    public enum DialogButtons
    {
        /// <summary>The dialog or message box contains and Ok button.</summary>
        Ok,
        /// <summary>The dialog or message box contains Ok and Cancel buttons.</summary>
        OkCancel,
        /// <summary>The dialog or message box contains Abort, Retry, and Ignore buttons.</summary>
        AbortRetryIgnore,
        /// <summary>The dialog or message box contains Yes, No, and Cancel buttons.</summary>
        YesNoCancel,
        /// <summary>The dialog or message box contains Yes and No buttons.</summary>
        YesNo,
        /// <summary>The dialog or message box contains Retry and Cancel buttons.</summary>
        RetryCancel
    }

    /// <summary>The base view model implementing the dialog host.</summary>
    /// <remarks>To customize the dialog host, subclass DialogHostBase and implement a custom view to match the subclassed view model.</remarks>
    [InheritedExport]
    [PartNotDiscoverable]
    public class DialogHostBase : Conductor<object>
    {
        private DialogButtons _dialogButtons;
        private IDialogHostDelegate _dialogHostDelegate;
        private DialogResult _dialogResult;

        static DialogHostBase()
        {
            ViewLocator.NameTransformer.AddRule("DialogHostBase$", "DialogHostView");
        }

        /// <summary>Indicates whether the Ok button should be enabled.</summary>
        public bool CanOk
        {
            get { return _dialogHostDelegate == null || _dialogHostDelegate.IsComplete; }
        }

        /// <summary>Indicates whether the Ok button should be visible.</summary>
        public bool ShowOk
        {
            get { return _dialogButtons == DialogButtons.Ok || _dialogButtons == DialogButtons.OkCancel; }
        }

        /// <summary>Indicates whether the Cancel button should be visible.</summary>
        public bool ShowCancel
        {
            get
            {
                return _dialogButtons == DialogButtons.OkCancel || _dialogButtons == DialogButtons.YesNoCancel ||
                       _dialogButtons == DialogButtons.RetryCancel;
            }
        }

        /// <summary>Indicates whether the Abort button should be visible.</summary>
        public bool ShowAbort
        {
            get { return _dialogButtons == DialogButtons.AbortRetryIgnore; }
        }

        /// <summary>Indicates whether the Retry button should be visible.</summary>
        public bool ShowRetry
        {
            get { return _dialogButtons == DialogButtons.AbortRetryIgnore || _dialogButtons == DialogButtons.RetryCancel; }
        }

        /// <summary>Indicates whether the Ignore button should be visible.</summary>
        public bool ShowIgnore
        {
            get { return _dialogButtons == DialogButtons.AbortRetryIgnore; }
        }

        /// <summary>Indicates that the Yes button should be visible.</summary>
        public bool ShowYes
        {
            get { return _dialogButtons == DialogButtons.YesNoCancel || _dialogButtons == DialogButtons.YesNo; }
        }

        /// <summary>Indicates whether the No button should be visible.</summary>
        public bool ShowNo
        {
            get { return _dialogButtons == DialogButtons.YesNo || _dialogButtons == DialogButtons.YesNoCancel; }
        }

        /// <summary>Returns the user's response to the dialog or message box.</summary>
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

        /// <summary>Signals that the user has responded to the dialog or message box.</summary>
        public event EventHandler<ResultCompletionEventArgs> Completed;

        /// <summary>Initializes and starts the dialog host.</summary>
        public DialogHostBase Start(string title, object content, DialogButtons dialogButtons)
        {
            _dialogHostDelegate = content as IDialogHostDelegate;
            if (_dialogHostDelegate != null)
            {
                _dialogHostDelegate.CompleteChanged -= CompleteChanged;
                _dialogHostDelegate.CompleteChanged += CompleteChanged;
            }

            DisplayName = title;
            _dialogButtons = dialogButtons;
            DialogResult = DialogResult.None;

            ActivateItem(content);
            return this;
        }

        private void CompleteChanged(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => CanOk);
        }

        /// <summary>The action invoked if the user clicks the Ok button.</summary>
        public void Ok()
        {
            TryClose(DialogResult.Ok);
        }

        /// <summary>The action invoked if the user clicks the Cancel button.</summary>
        public void Cancel()
        {
            TryClose(DialogResult.Cancel);
        }

        /// <summary>The action invoke if the user clicks the Abort button.</summary>
        public void Abort()
        {
            TryClose(DialogResult.Abort);
        }

        /// <summary>The action invoked if the user click's the Retry button.</summary>
        public void Retry()
        {
            TryClose(DialogResult.Retry);
        }

        /// <summary>The action invoked if the user click's the Ignore button.</summary>
        public void Ignore()
        {
            TryClose(DialogResult.Ignore);
        }

        /// <summary>The action invoked if the user click's the Yes button.</summary>
        public void Yes()
        {
            TryClose(DialogResult.Yes);
        }

        /// <summary>The action invoked if the user clicks the No button.</summary>
        public void No()
        {
            TryClose(DialogResult.No);
        }

        /// <summary>Close the dialog host and specify the user's response.</summary>
        protected void TryClose(DialogResult dialogResult)
        {
            DialogResult = dialogResult;
            TryClose();
        }

        /// <summary>Internal use.</summary>
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

        /// <summary>Raises the <see cref="Completed"/> event.</summary>
        protected void OnComplete()
        {
            if (Completed == null) return;

            var args = new ResultCompletionEventArgs { WasCancelled = DialogResult == DialogResult.Cancel };
            EventFns.RaiseOnce(ref Completed, this, args);
        }
    }
}