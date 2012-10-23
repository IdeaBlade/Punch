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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cocktail
{
    /// <summary>A service that manages modal dialogs and message boxes.</summary>
    public class DialogManager : IDialogManager
    {
        #region IDialogManager Members

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>The dialog result.</returns>
        public Task<T> ShowDialogAsync<T>(object content, IEnumerable<T> dialogButtons, string title)
        {
            var dialog = new Dialog<T>(content, dialogButtons, title);
            return dialog.Show();
        }

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="defaultButton"> 
        /// Specifies the default button. The Enter key will be mapped to this button.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the button taking on the special role of the cancel function. If the user clicks this button, 
        /// the Task will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>The dialog result.</returns>
        public Task<T> ShowDialogAsync<T>(object content, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
        {
            var dialog = new Dialog<T>(content, dialogButtons, defaultButton, cancelButton, title);
            return dialog.Show();
        }

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <returns>The dialog result.</returns>
        public Task<DialogResult> ShowDialogAsync(object content, IEnumerable<DialogResult> dialogButtons, string title = null)
        {
            var dialog = new Dialog<DialogResult>(content, dialogButtons, DialogResult.Ok, DialogResult.Cancel, title);
            return dialog.Show();
        }

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>The dialog result.</returns>
        public Task<T> ShowMessageAsync<T>(string message, IEnumerable<T> dialogButtons, string title)
        {
            var messageBox = CreateMessageBox(message);
            var dialog = new Dialog<T>(messageBox, dialogButtons, title);
            return dialog.Show();
        }

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="defaultButton"> 
        /// Specifies the default button. The Enter key will be mapped to this button.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the button taking on the special role of the cancel function. If the user clicks this button, 
        /// the Task will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>The dialog result.</returns>
        public Task<T> ShowMessageAsync<T>(string message, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
        {
            var messageBox = CreateMessageBox(message);
            var dialog = new Dialog<T>(messageBox, dialogButtons, defaultButton, cancelButton, title);
            return dialog.Show();
        }

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <returns>The dialog result.</returns>
        public Task<DialogResult> ShowMessageAsync(string message, IEnumerable<DialogResult> dialogButtons, string title = null)
        {
            var messageBox = CreateMessageBox(message);
            var dialog = new Dialog<DialogResult>(messageBox, dialogButtons, DialogResult.Ok, DialogResult.Cancel, title);
            return dialog.Show();
        }

        #endregion

        private MessageBoxBase CreateMessageBox(string message)
        {
            var messageBoxLocator = new PartLocator<MessageBoxBase>(true)
                .WithDefaultGenerator(() => new MessageBoxBase());
            return messageBoxLocator.GetPart().Start(message);
        }
    }
}