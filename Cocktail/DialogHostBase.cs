// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using Caliburn.Micro;
using IdeaBlade.Core.Composition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

#if !NETFX_CORE
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail
{
    /// <summary>
    ///   The base view model implementing the dialog host.
    /// </summary>
    /// <remarks>
    ///   To customize the dialog host, subclass DialogHostBase and implement a custom view to match the subclassed view model.
    /// </remarks>
    [InterfaceExport(typeof(DialogHostBase))]
    [PartNotDiscoverable]
    public class DialogHostBase : Conductor<object>, IDialogHost
    {
        private object _cancelButton;
        private object _defaultButton;
        private IEnumerable<DialogButton> _dialogButtons;
        private object _dialogResult;
        private bool _isClosing;

        static DialogHostBase()
        {
            ViewLocator.NameTransformer.AddRule("DialogHostBase$", "DialogHostView");
        }

        /// <summary>
        /// Initializes a new DialogHostBase instance.
        /// </summary>
        public DialogHostBase()
        {
            CloseStrategy = new DialogClosingStrategy(this);
        }

        /// <summary>
        ///   Contains the list of buttons to be displayed.
        /// </summary>
        public IEnumerable<DialogButton> DialogButtons
        {
            get { return _dialogButtons; }
            set
            {
                _dialogButtons = value;
                NotifyOfPropertyChange(() => DialogButtons);
            }
        }

        /// <summary>
        /// Indicates whether the dialog actions (buttons) should be enabled or disabled.
        /// </summary>
        public bool ActionsEnabled
        {
            get { return !IsClosing; }
        }

        #region IDialogHost Members

        /// <summary>
        ///   Returns the user's response to the dialog or message box.
        /// </summary>
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

        void IDialogHost.TryClose(object dialogResult)
        {
            if (IsClosing) return;

            IsClosing = true;
            DialogResult = dialogResult;
            TryClose();
        }

        #endregion

        /// <summary>
        ///   Signals that the user has responded to the dialog or message box.
        /// </summary>
        internal event EventHandler<EventArgs> Completed;

        /// <summary>
        ///   Initializes and starts the dialog host.
        /// </summary>
        public DialogHostBase Start(string title, object content, IEnumerable dialogButtons, object defaultButton,
                                    object cancelButton)
        {
            DisplayName = title;
            _dialogButtons =
                new ObservableCollection<DialogButton>(dialogButtons.Cast<object>().Select(v => new DialogButton(v)));
            _defaultButton = defaultButton;
            _cancelButton = cancelButton;

            ActivateItem(content);
            return this;
        }

        /// <summary>
        ///   Action invoked when the user clicks on any dialog or message box button.
        /// </summary>
        /// <param name="dialogButton"> The button that was clicked. </param>
        public void Close(DialogButton dialogButton)
        {
            ((IDialogHost)this).TryClose(dialogButton.Value);
        }

        /// <summary>
        ///   Internal use.
        /// </summary>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                OnComplete();
        }

        /// <summary>
        ///   Raises the <see cref="Completed" /> event.
        /// </summary>
        protected void OnComplete()
        {
            if (Completed == null) return;

            EventFns.RaiseOnce(ref Completed, this, EventArgs.Empty);
        }

        /// <summary>
        ///   Called when DialogHostView's Loaded event fires.
        /// </summary>
        /// <param name="view" />
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ((UIElement)view).KeyDown += (sender, args) => OnKeyDown(args);
        }

        /// <summary>
        ///   Called to check whether the dialog host can be closed.
        /// </summary>
        public override void CanClose(Action<bool> callback)
        {
            if (DialogResult != null)
            {
                base.CanClose(callback);
                return;
            }

            if (_cancelButton != null)
            {
                var button = _dialogButtons.FirstOrDefault(b => b.Value.Equals(_cancelButton));
                if (button == null || !button.Enabled)
                    return;
                DialogResult = button.Value;
                base.CanClose(callback);
                return;
            }

            callback(false);
        }

        private void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.Escape && _cancelButton != null)
            {
                var button = _dialogButtons.FirstOrDefault(b => b.Value.Equals(_cancelButton));
                if (button == null || !button.Enabled)
                    return;
                Close(button);
            }

            if (args.Key == Key.Enter && _defaultButton != null)
            {
                var button = _dialogButtons.FirstOrDefault(b => b.Value.Equals(_defaultButton));
                if (button == null || !button.Enabled)
                    return;
                Close(button);
            }
        }

        private bool IsClosing
        {
            get { return _isClosing; }
            set
            {
                _isClosing = value;
                NotifyOfPropertyChange(() => ActionsEnabled);
            }
        }

        #region Nested type: DialogClosingStrategy

        private class DialogClosingStrategy : ICloseStrategy<object>
        {
            private readonly ICloseStrategy<object> _defaultStrategy = new DefaultCloseStrategy<object>();
            private readonly DialogHostBase _dialogHost;

            public DialogClosingStrategy(DialogHostBase dialogHost)
            {
                _dialogHost = dialogHost;
            }

            #region ICloseStrategy<object> Members

            public void Execute(IEnumerable<object> toClose, Action<bool, IEnumerable<object>> callback)
            {
                _defaultStrategy.Execute(toClose, (canClose, closable) =>
                                                      {
                                                          callback(canClose, closable);

                                                          if (canClose) return;

                                                          _dialogHost.IsClosing = false;
                                                          _dialogHost.DialogResult = null;
                                                      });
            }

            #endregion
        }

        #endregion
    }
}