namespace Common.Dialog
{
    public class ShowMessageResult : ShowDialogResult
    {
        public ShowMessageResult(string message, bool hideCancel = true, string title = "")
            : base(new MessageViewModel(message), hideCancel, title)
        {
        }
    }
}