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

using System.Collections.Generic;
using System.ComponentModel.Composition;

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
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<T> ShowDialog<T>(object content, IEnumerable<T> dialogButtons, string title)
        {
            var result = new ShowDialogResult<T>(content, dialogButtons, title);
            result.Show();
            return result;
        }

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the button taking on the special role of the cancel function. If the user clicks this button, 
        /// the DialogOperationResult will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<T> ShowDialog<T>(object content, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
        {
            var result = new ShowDialogResult<T>(content, dialogButtons, cancelButton, title);
            result.Show();
            return result;
        }

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<DialogResult> ShowDialog(object content, IEnumerable<DialogResult> dialogButtons, string title = null)
        {
            var result = new ShowDialogResult<DialogResult>(content, dialogButtons, DialogResult.Cancel, title);
            result.Show();
            return result;
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
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<T> ShowMessage<T>(string message, IEnumerable<T> dialogButtons, string title)
        {
            var messageBox = CreateMessageBox(message);
            var result = new ShowDialogResult<T>(messageBox, dialogButtons, title);
            result.Show();
            return result;
        }

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the button taking on the special role of the cancel function. If the user clicks this button, 
        /// the DialogOperationResult will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<T> ShowMessage<T>(string message, T cancelButton, IEnumerable<T> dialogButtons, string title = null)
        {
            var messageBox = CreateMessageBox(message);
            var result = new ShowDialogResult<T>(messageBox, dialogButtons, cancelButton, title);
            result.Show();
            return result;
        }

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        public DialogOperationResult<DialogResult> ShowMessage(string message, IEnumerable<DialogResult> dialogButtons, string title = null)
        {
            var messageBox = CreateMessageBox(message);
            var result = new ShowDialogResult<DialogResult>(messageBox, dialogButtons, DialogResult.Cancel, title);
            result.Show();
            return result;
        }

        #endregion

        private MessageBoxBase CreateMessageBox(string message)
        {
            var messageBoxLocator = new PartLocator<MessageBoxBase>(CreationPolicy.NonShared)
                .WithDefaultGenerator(() => new MessageBoxBase());
            return messageBoxLocator.GetPart().Start(message);
        }
    }
}