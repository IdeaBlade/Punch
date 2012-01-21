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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>The base view model implementing the dialog host.</summary>
    /// <remarks>To customize the dialog host, subclass DialogHostBase and implement a custom view to match the subclassed view model.</remarks>
    [InheritedExport]
    [PartNotDiscoverable]
    public class DialogHostBase : Conductor<object>, IDialogHost
    {
        private IEnumerable<DialogButton> _dialogButtons;
        private object _dialogResult;

        static DialogHostBase()
        {
            ViewLocator.NameTransformer.AddRule("DialogHostBase$", "DialogHostView");
        }

        /// <summary>Contains the list of buttons to be displayed.</summary>
        public IEnumerable<DialogButton> DialogButtons
        {
            get { return _dialogButtons; }
            set
            {
                _dialogButtons = value;
                NotifyOfPropertyChange(() => DialogButtons);
            }
        }

        #region IDialogHost Members

        /// <summary>Returns the user's response to the dialog or message box.</summary>
        public object DialogResult
        {
            get { return _dialogResult; }
            private set
            {
                _dialogResult = value;
                NotifyOfPropertyChange(() => DialogResult);
            }
        }

        DialogButton IDialogHost.GetButton(object value)
        {
            return DialogButtons.First(b => b.Value.Equals(value));
        }

        #endregion

        /// <summary>Signals that the user has responded to the dialog or message box.</summary>
        internal event EventHandler<EventArgs> Completed;

        /// <summary>Initializes and starts the dialog host.</summary>
        public DialogHostBase Start(string title, object content, IEnumerable dialogButtons)
        {
            DisplayName = title;
            _dialogButtons =
                new ObservableCollection<DialogButton>(dialogButtons.Cast<object>().Select(v => new DialogButton(v)));

            ActivateItem(content);
            return this;
        }

        /// <summary>Action invoked when the user clicks on any dialog or message box button.</summary>
        /// <param name="dialogButton">The button that was clicked.</param>
        public void Close(DialogButton dialogButton)
        {
            DialogResult = dialogButton.Value;
            TryClose();
        }

        /// <summary>Internal use.</summary>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                OnComplete();
                ActiveItem = null;
            }

            base.OnDeactivate(close);
        }

        /// <summary>Raises the <see cref="Completed"/> event.</summary>
        protected void OnComplete()
        {
            if (Completed == null) return;

            EventFns.RaiseOnce(ref Completed, this, EventArgs.Empty);
        }
    }
}