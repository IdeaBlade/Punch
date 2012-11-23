//  ====================================================================================================================
//    Copyright (c) 2012 IdeaBlade
//  ====================================================================================================================
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//    WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//    OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//    OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//  ====================================================================================================================
//    USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//    http://cocktail.ideablade.com/licensing
//  ====================================================================================================================

using System;
using System.ComponentModel;

namespace Cocktail
{
    /// <summary>
    ///     Represents a command in the UI.
    /// </summary>
    public abstract class UICommand : IUICommand, INotifyPropertyChanged, IDisposable
    {
        private bool _enabled = true;
        private UICommandInvokedArgs _args;

        /// <summary>
        ///     Creates a new instance of the UICommand class.
        /// </summary>
        /// <param name="label">The label of the command.</param>
        /// <param name="isDefaultCommand">True if command is the default command.</param>
        /// <param name="isCancelCommand">True if the command is used to cancel.</param>
        protected UICommand(string label, bool isDefaultCommand = false, bool isCancelCommand = false)
            : this(isDefaultCommand, isCancelCommand)
        {
            Label = label;
        }

        /// <summary>
        ///     Creates a new instance of the UICommand class.
        /// </summary>
        /// <param name="isDefaultCommand">True if command is the default command.</param>
        /// <param name="isCancelCommand">True if the command is used to cancel.</param>
        protected UICommand(bool isDefaultCommand = false, bool isCancelCommand = false)
        {
            IsDefaultCommand = isDefaultCommand;
            IsCancelCommand = isCancelCommand;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            Invoked = null;
            PropertyChanged = null;
        }

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets or sets the optional label of the UI command.
        /// </summary>
        public string Label { get; protected set; }

        /// <summary>
        ///     Specifies whether this command is the default command.
        /// </summary>
        public bool IsDefaultCommand { get; protected set; }

        /// <summary>
        ///     Specifies whether this command is used to cancel an operation.
        /// </summary>
        public bool IsCancelCommand { get; protected set; }

        /// <summary>
        ///     Enables or disables the UI command.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Returns true if the command invocation was cancelled.
        /// </summary>
        public bool WasCancelled { get { return _args != null && _args.Cancelled; } }

        /// <summary>
        ///     Event triggerd when the UI command is being invoked.
        /// </summary>
        public event EventHandler<UICommandInvokedArgs> Invoked;

        /// <summary>
        ///     Triggers the <see cref="UICommand.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Triggers the <see cref="Invoked"/> event.
        /// </summary>
        /// <returns>True if the command was successful, false if the command was cancelled.</returns>
        protected virtual bool OnInvoked(UICommandInvokedArgs args)
        {
            _args = args;
            var handler = Invoked;
            if (handler != null) handler(this, args);

            return !args.Cancelled;
        }
    }

    /// <summary>
    /// Provides information to the <see cref="UICommand.Invoked"/> event.
    /// </summary>
    public class UICommandInvokedArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the UICommandInvokedArgs class.
        /// </summary>
        /// <param name="command">The invoked command.</param>
        public UICommandInvokedArgs(IUICommand command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets the invoked command.
        /// </summary>
        public IUICommand Command { get; private set; }

        /// <summary>
        /// True if the command was cancelled.
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// Cancels the command.
        /// </summary>
        public void Cancel()
        {
            Cancelled = true;
        }
    }
}