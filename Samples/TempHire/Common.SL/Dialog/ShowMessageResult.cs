namespace Common.Dialog
{
    public class ShowMessageResult : ShowDialogResult
    {
        internal ShowMessageResult(string message, bool hideCancel = true, string title = "")
            : base(new MessageViewModel(message), hideCancel, title)
        {
        }
    }
}