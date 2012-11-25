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

using System;

namespace Cocktail
{
    internal interface IHasDialogResult
    {
        object DialogResult { get; }
    }

    internal interface IInvokeCommand
    {
        bool Invoke(IDialogHost dialogHost);
    }

    /// <summary>
    ///     Represents a command in a message or dialog box.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DialogUICommand<T> : UICommand, IDialogUICommand<T>, IHasDialogResult, IInvokeCommand
    {
        /// <summary>
        ///     Creates a new instance of the DialogUICommand&lt;T&gt; class.
        /// </summary>
        /// <param name="label">The label of the command.</param>
        /// <param name="dialogResult">The associated dialog result value.</param>
        /// <param name="isDefaultCommand">True if command is the default command.</param>
        /// <param name="isCancelCommand">True if the command is used to cancel.</param>
        public DialogUICommand(string label, T dialogResult, bool isDefaultCommand = false, bool isCancelCommand = false)
            : base(label, isDefaultCommand, isCancelCommand)
        {
            DialogResult = dialogResult;
        }

        /// <summary>
        ///     Creates a new instance of the DialogUICommand&lt;T&gt; class.
        /// </summary>
        /// <param name="dialogResult">The associated dialog result value.</param>
        /// <param name="isDefaultCommand">True if command is the default command.</param>
        /// <param name="isCancelCommand">True if the command is used to cancel.</param>
        public DialogUICommand(T dialogResult, bool isDefaultCommand = false, bool isCancelCommand = false)
            : base(isDefaultCommand, isCancelCommand)
        {
            DialogResult = dialogResult;
        }

        /// <summary>
        ///     The dialog result associated with this command.
        /// </summary>
        public T DialogResult { get; private set; }

        /// <summary>
        ///     Event triggerd when the UI command is being invoked.
        /// </summary>
        public new event EventHandler<DialogUICommandInvokedArgs> Invoked;

        object IHasDialogResult.DialogResult
        {
            get { return DialogResult; }
        }

        bool IInvokeCommand.Invoke(IDialogHost dialogHost)
        {
            var args = new DialogUICommandInvokedArgs(dialogHost, this);
            return OnInvoked(args);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();
            Invoked = null;
        }

        /// <summary>
        ///     Triggers the <see cref="Invoked" /> event.
        /// </summary>
        /// <returns>True if the command was successful, false if the command was cancelled.</returns>
        protected virtual bool OnInvoked(DialogUICommandInvokedArgs args)
        {
            OnInvoked((UICommandInvokedArgs) args);

            var handler = Invoked;
            if (handler != null) handler(this, args);

            return !args.Cancelled;
        }

        private new void OnInvoked(UICommandInvokedArgs args)
        {
            base.OnInvoked(args);
        }
    }

    /// <summary>
    ///     Provides information to the <see cref="DialogUICommand{T}.Invoked" /> event.
    /// </summary>
    public class DialogUICommandInvokedArgs : UICommandInvokedArgs
    {
        /// <summary>
        ///     Creates a new instance of the DialogUICommandInvokedArgs class.
        /// </summary>
        /// <param name="dialogHost">The dialog host associated with the command.</param>
        /// <param name="command">The invoked command.</param>
        public DialogUICommandInvokedArgs(IDialogHost dialogHost, IUICommand command)
            : base(command)
        {
            DialogHost = dialogHost;
        }

        /// <summary>
        ///     Returns the dialog host associated with the command.
        /// </summary>
        public IDialogHost DialogHost { get; private set; }
    }
}