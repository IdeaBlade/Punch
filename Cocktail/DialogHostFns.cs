using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>A collection of extension methods to interact with the dialog host.</summary>
    public static class DialogHostFns
    {
        /// <summary>Returns a reference to the dialog host if the provided ViewModel is currently hosted in the dialog host.</summary>
        /// <param name="source">The hosted ViewModel.</param>
        /// <returns>Null if the ViewModel is not currently hosted in a dialog host, otherwise a reference to the current dialog host.</returns>
        public static IDialogHost DialogHost(this IChild source)
        {
            return source.Parent as IDialogHost;
        }
    }
}