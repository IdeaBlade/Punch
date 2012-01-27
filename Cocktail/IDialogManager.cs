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

namespace Cocktail
{
    /// <summary>Represents a user's response to a dialog or message box.</summary>
    public enum DialogResult
    {
        /// <summary>Nothing. This means that the dialog or message box continues running.</summary>
        None,

        /// <summary>The user clicked the Ok button.</summary>
        Ok,

        /// <summary>The user clicked the Cancel button.</summary>
        Cancel,

        /// <summary>The user clicked the Abort button.</summary>
        Abort,

        /// <summary>The user clicked the Retry button.</summary>
        Retry,

        /// <summary>The user clicked the Ignore button.</summary>
        Ignore,

        /// <summary>The user clicked the Yes button.</summary>
        Yes,

        /// <summary>The user clicked the No button.</summary>
        No
    };

    /// <summary>Specifies constants defining which buttons to display on a dialog or message box.</summary>
    public static class DialogButtons
    {
        /// <summary>The dialog or message box contains and Ok button.</summary>
        public static readonly IEnumerable<DialogResult> Ok = new[] { DialogResult.Ok };

        /// <summary>The dialog or message box contains Ok and Cancel buttons.</summary>
        public static readonly IEnumerable<DialogResult> OkCancel = new[] { DialogResult.Ok, DialogResult.Cancel };

        /// <summary>The dialog or message box contains Abort, Retry, and Ignore buttons.</summary>
        public static readonly IEnumerable<DialogResult> AbortRetryIgnore
            = new[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore };

        /// <summary>The dialog or message box contains Yes, No, and Cancel buttons.</summary>
        public static readonly IEnumerable<DialogResult> YesNoCancel
            = new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };

        /// <summary>The dialog or message box contains Yes and No buttons.</summary>
        public static readonly IEnumerable<DialogResult> YesNo = new[] { DialogResult.Yes, DialogResult.No };

        /// <summary>The dialog or message box contains Retry and Cancel buttons.</summary>
        public static readonly IEnumerable<DialogResult> RetryCancel = new[] { DialogResult.Retry, DialogResult.Cancel };
    }

    /// <summary>A service that manages modal dialogs and message boxes.</summary>
    public interface IDialogManager
    {
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
        DialogOperationResult<T> ShowDialog<T>(object content, IEnumerable<T> dialogButtons, string title = null);

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the designated cancel button. If the user clicks this button, the DialogOperationResult will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        DialogOperationResult<T> ShowDialog<T>(object content, T cancelButton, IEnumerable<T> dialogButtons,
                                               string title = null);

        /// <summary>Displays a modal dialog with a custom view model.</summary>
        /// <param name="content">The custom view model to host in the dialog.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the dialog.</param>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        DialogOperationResult<DialogResult> ShowDialog(object content, IEnumerable<DialogResult> dialogButtons,
                                                       string title = null);

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
        DialogOperationResult<T> ShowMessage<T>(string message, IEnumerable<T> dialogButtons, string title = null);

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="cancelButton">
        /// Specifies the designated cancel button. If the user clicks this button, the DialogOperationResult will be marked as cancelled.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <typeparam name="T">
        /// User-defined dialog result type. In most cases <see cref="object.ToString()"/> is used as the button content.
        /// </typeparam>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        DialogOperationResult<T> ShowMessage<T>(string message, T cancelButton, IEnumerable<T> dialogButtons,
                                                string title = null);

        /// <summary>Displays a modal message box.</summary>
        /// <param name="message">The message to display.</param>
        /// <param name="dialogButtons">
        /// A value that indicates the button or buttons to display. See <see cref="DialogButtons"/> for predefined button sets.
        /// </param>
        /// <param name="title">Optional title of the message box.</param>
        /// <returns>A value representing the asynchronous operation of displaying the dialog.</returns>
        DialogOperationResult<DialogResult> ShowMessage(string message, IEnumerable<DialogResult> dialogButtons,
                                                        string title = null);
    }
}