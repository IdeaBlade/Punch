//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>Represents an individual button displayed by the DialogHost.</summary>
    public class DialogButton : PropertyChangedBase, IDisposable
    {
        private readonly IUICommand _command;
        private readonly IDialogHost _dialogHost;

        /// <summary>Initializes a new instance of DialogButton.</summary>
        /// <param name="command">The associated UI command.</param>
        /// <param name="dialogHost">The assoicated dialog host.</param>
        internal DialogButton(IUICommand command, IDialogHost dialogHost)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (dialogHost == null) throw new ArgumentNullException("dialogHost");
            Debug.Assert(command is IHasDialogResult);
            Debug.Assert(command is INotifyPropertyChanged);
            Debug.Assert(command is IInvokeCommand);

            _command = command;
            _dialogHost = dialogHost;
            ((INotifyPropertyChanged) _command).PropertyChanged += CommandPropertyChanged;
        }

        internal object DialogResult
        {
            get { return ((IHasDialogResult) _command).DialogResult; }
        }

        /// <summary>
        /// Returns the command associated with this button.
        /// </summary>
        public IUICommand Command
        {
            get { return _command; }
        }

        /// <summary>The button content displayed in the view.</summary>
        public object Content
        {
            get
            {
                if (!string.IsNullOrEmpty(_command.Label))
                    return _command.Label;

                if (DialogResult is DialogResult)
                    return StringResources.ResourceManager.GetString(DialogResult.ToString());
                return DialogResult;
            }
        }

        /// <summary>Indicates whether the button is currently enabled or disabled.</summary>
        public bool Enabled
        {
            get { return _command.Enabled; }
            set { _command.Enabled = value; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            var disposable = _command as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            ((INotifyPropertyChanged) _command).PropertyChanged -= CommandPropertyChanged;
        }

        internal bool InvokeCommand()
        {
            return ((IInvokeCommand) _command).Invoke(_dialogHost);
        }

        private void CommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled")
                NotifyOfPropertyChange(() => Enabled);
        }
    }
}