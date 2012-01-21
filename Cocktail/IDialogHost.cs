namespace Cocktail
{
    /// <summary>Public interface to interact with the dialog host.</summary>
    public interface IDialogHost
    {
        /// <summary>Returns the user's response to a dialog or message box.</summary>
        object DialogResult { get; }

        /// <summary>Returns the logical button for the provided button value.</summary>
        /// <param name="value">The user response value associated with this button.</param>
        /// <returns>A logical object representing the dialog or message box button.</returns>
        DialogButton GetButton(object value);
    }
}